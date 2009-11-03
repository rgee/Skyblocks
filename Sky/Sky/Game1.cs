using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Sky.PhysicsEntities;

namespace Sky
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

       
        FirstPersonCamera camera;

        Model skyBoxModel;
        TextureCube skyboxTexture;

        Skydome skybox;

        Player ship;
        Model shipModel;
        Effect diffuseFX;

        CubeEntity[] cubes;
        Model blockModel;

        PlaneEntity floor;
        Model floorModel;

        public FirstPersonCamera Camera
        {
            get { return camera; }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            camera = new FirstPersonCamera(this);
            Components.Add(camera);

            InitializePhysics();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            // Disable vertical retrace to get highest framerates possible for
            // testing performance.
            graphics.SynchronizeWithVerticalRetrace = false;

            // Set minimum pixel and vertex shader requirements.
            graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
            graphics.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;
            
             
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            skyBoxModel = Content.Load<Model>("Models/skybox");
            skyboxTexture = Content.Load<TextureCube>("Textures/skybox02");
            shipModel = Content.Load<Model>("Models/PlayerMarine");
            floorModel = Content.Load<Model>("Models/grid");
            diffuseFX = Content.Load<Effect>("Effects/Specular");
            blockModel = Content.Load<Model>("Models/Cube");

            skybox = new Skydome();
            skybox.LoadContent(GraphicsDevice, skyBoxModel, skyboxTexture);

            floor = new PlaneEntity(this, floorModel, 15.0f);
            Components.Add(floor);
            // Code for initializing the ring of cubes. Will be moved to an appropriate
            // location soon.
            Matrix[] cubeWorlds = new Matrix[6];
            for (int i = 0; i < 6; i++)
            {
                float theta = MathHelper.TwoPi * ((float) i / 8f);
                cubeWorlds[i] = Matrix.CreateTranslation(
                    500f * (float)Math.Sin(theta), 0, 500f * (float)Math.Cos(theta));
            }
            cubes = new CubeEntity[6];
            for (int i = 0; i < 6; i++)
            {
                cubes[i] = new CubeEntity(this, blockModel, new Vector3(1, 1, 1), Matrix.Identity, new Vector3(0, 50 + i, 20));
               
                Components.Add(cubes[i]);
            }

            ship = new Player(graphics.GraphicsDevice);
            ship.LoadContent(shipModel, diffuseFX, Content);

            
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();



            MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();

            
            //ship.Update(gameTime);

            //camera.Update(gameTime);

            float timeStep = (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
            PhysicsSystem.CurrentPhysicsSystem.Integrate(timeStep);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            skybox.Draw(camera);
            
            ship.Draw(camera.View, camera.Projection);


            base.Draw(gameTime);
        }

        /// <summary>
        /// Initialize the JigLibX physics system. Used for testing.
        /// </summary>
        private void InitializePhysics()
        {
            PhysicsSystem world = new PhysicsSystem();
            world.CollisionSystem = new CollisionSystemSAP();
        }
    }
}
