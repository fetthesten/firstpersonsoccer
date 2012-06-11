using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.DataStructures;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Collidables;
using fpsoccer.InputWrappers;

namespace fpsoccer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FpsGame : Game
    {
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        // physics engine stuff
        Space _space;
        public Model TestModel;
        public Model TestPitch;

        // keyboard/mouse controls
        private readonly KeyboardWrapper _keyboard = new KeyboardWrapper();
        private readonly MouseWrapper _mouse = new MouseWrapper();
        private readonly InvertOptions _invertOptions = new InvertOptions();

        // camera control
        public Camera Camera;

        // renderer
        Effect _effect;

        public FpsGame()
        {
            DisplayMode currentDisplayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = currentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = currentDisplayMode.Height;
            // graphics.ToggleFullScreen();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Camera = new Camera(this, new Vector3(0, -25.0f, 10.0f), 5.0f);
            Window.Title = "First Person Soccer";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // load models
            TestModel = Content.Load<Model>("mdl/football");
            TestPitch = Content.Load<Model>("mdl/pitch");

            // initialise physics simulation space
            _space = new Space();
            _space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);
            Vector3[] vertices;
            int[] indices;

            TriangleMesh.GetVerticesAndIndicesFromModel(TestPitch, out vertices, out indices);
            var mesh = new StaticMesh(vertices, indices, new BEPUphysics.MathExtensions.AffineTransform(new Vector3(0, -40.0f, 0)));
            _space.Add(mesh);
            Components.Add(new StaticModel(TestPitch, mesh.WorldTransform.Matrix, this));

            _space.Add(new Sphere(new Vector3(0, 4, 0), 1, 1));
            
            // test; adding a few entities to space
            foreach (Entity e in _space.Entities)
            {
                Sphere sphere = e as Sphere;
                if (sphere != null)
                {
                    Matrix scaling = Matrix.CreateScale(sphere.Radius);
                    EntityModel model = new EntityModel(e, TestModel, scaling, this);
                    Components.Add(model);
                    e.Tag = model;
                }
            }
        }

        /// <summary>
        /// Constructs a mesh based on colour data from a texture
        /// </summary>
        /// <param name="heightMap">Texture to construct mesh from</param>
        /// <param name="indices">Int array of indices</param>
        /// <param name="vertices">Vector3 array of vertices</param>
        private void LoadHeightData(Texture2D heightMap, out Vector3[] vertices, out int[] indices)
        {
            float minimumHeight = float.MaxValue;
            float maximumHeight = float.MinValue;

            int mapWidth = heightMap.Width;
            int mapHeight = heightMap.Height;

            Color[] heightMapColors = new Color[mapWidth * mapHeight];
            heightMap.GetData(heightMapColors);

            float[,] heightData = new float[mapWidth, mapHeight];
            for (int x = 0; x < mapWidth; x++)
                for (int y = 0; y < mapHeight; y++)
                {
                    heightData[x, y] = heightMapColors[x + y * mapWidth].R;
                    if (heightData[x, y] < minimumHeight) minimumHeight = heightData[x, y];
                    if (heightData[x, y] > maximumHeight) maximumHeight = heightData[x, y];
                }

            for (int x = 0; x < mapWidth; x++)
                for (int y = 0; y < mapHeight; y++)
                    heightData[x, y] = (heightData[x, y] - minimumHeight) / (maximumHeight - minimumHeight) * 30.0f;

            vertices = new Vector3[mapWidth * mapHeight];

            for (int x = 0; x < mapWidth; x++)
                for (int y = 0; y < mapHeight; y++)
                    vertices[x + y * mapWidth] = new Vector3(x, heightData[x, y], -y);

            indices = new int[(mapWidth - 1) * (mapHeight - 1) * 6];
            int counter = 0;
            for (int y = 0; y < mapHeight - 1; y++)
            {
                for (int x = 0; x < mapWidth - 1; x++)
                {
                    int lowerLeft = x + y * mapWidth;
                    int lowerRight = (x + 1) + y * mapWidth;
                    int topLeft = x + (y + 1) * mapWidth;
                    int topRight = (x + 1) + (y + 1) * mapWidth;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }
        }

        /// <summary>
        /// Handle collision event triggered by entity
        /// </summary>
        /// <param name="sender">Entity that had an event hooked</param>
        /// <param name="other">Entity causing event to be triggered</param>
        /// <param name="pair">Collision pair between the two objects in the event</param>
        void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            var otherEntityInformation = other as EntityCollidable;
            if (otherEntityInformation != null)
            {
                // hit an entity, remove it
                _space.Remove(otherEntityInformation.Entity);
                // also remove graphics
                Components.Remove((EntityModel)otherEntityInformation.Entity.Tag);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // update keyboard and mouse state
            _keyboard.Update();
            _mouse.Update();
            
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || _keyboard.State.IsKeyDown(Keys.Escape))
                this.Exit();

            // update camera
            Camera.Update((float)gameTime.ElapsedGameTime.TotalSeconds, _keyboard, _mouse);

            _space.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
