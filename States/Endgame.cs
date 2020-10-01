﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameV2.Controls;
namespace MonoGameV2.States
{
    public class Endgame : State
    {
        private List<Component> _components;

        public Endgame(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            var buttonTexture = _content.Load<Texture2D>("Button");
            var buttonFont = _content.Load<SpriteFont>("ButtonFont");

            var RestartGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(330, 250),
                Text = "Play Again",
            };

            RestartGameButton.Click += RestartGameButton_Click;

            var MenuGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(330, 300),
                Text = "main Menu",
            };

            MenuGameButton.Click += MenuGameButton_Click;

            var quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(330, 350),
                Text = "Quit Game",
            };

            quitGameButton.Click += QuitGameButton_Click;

            _components = new List<Component>()
      {
        RestartGameButton,
        MenuGameButton,
        quitGameButton,
      };

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        private void RestartGameButton_Click(object sender, EventArgs e)
        {
            _game.changeState(new GameState(_game, _graphicsDevice, _content));
        }

        private void MenuGameButton_Click(object sender, EventArgs e)
        {
            _game.changeState(new MenuState(_game, _graphicsDevice, _content));
        }

        public override void PostUpdate(GameTime gameTime)
        {
            //removsprites
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            _game.Exit();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }
    }
}
