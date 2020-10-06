﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameV2.Sprites;
using System;
using MonoGameV2.Controls;
using MonoGameV2.Constants;

namespace MonoGameV2.States
{
    public class GameState : State
    {
        private SpriteBatch _spriteBatch;

        public static int screenWidth = Game1.screenWidth;
        public static int screenHeight = Game1.screenHeight;
        public static Random random = Game1.random;
        private Ball ball;
        private AIPlayer AIplayer;
        private Player Player1;
        private List<Sprite> sprites;
        private List<Component> _components, _componentsPause;
        private Score score;
        private ScreenText screenFont;
        private int difficultyCase;
        private float currentYPosition;
        private bool Pause, Multiplayer;

    public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
    : base(game, graphicsDevice, content)
    {
        difficultyCase = 0;
        _spriteBatch = new SpriteBatch(_graphicsDevice);
        //use this.Content to load your game content here
        var ballTexture = _content.Load<Texture2D>("Ball");
        var playerTexture = _content.Load<Texture2D>("Player1");
        var player2Texture = _content.Load<Texture2D>("Player2");
        var buttonTexture = _content.Load<Texture2D>("Home");
        var buttonFont = _content.Load<SpriteFont>("ButtonFont");
        var DifficultybuttonTexture = _content.Load<Texture2D>("difficultyButton");
        var DifficultybuttonFont = _content.Load<SpriteFont>("ButtonDifficultyFont");
        var pausebuttonTexture = _content.Load<Texture2D>("Pause2");

        score = new Score(_content.Load<SpriteFont>("File"));
        screenFont = new ScreenText(_content.Load<SpriteFont>("ScreenFont"));

        var HomeButton = new Button(buttonTexture, buttonFont)
        {
            Position = new Vector2(15, 450),
            Text = "",
        };
        HomeButton.Click += HomeButton_Click;

        var PauseButton = new Button(pausebuttonTexture, buttonFont)
        {
            Position = new Vector2(screenWidth/2 - pausebuttonTexture.Width/2, 452),
            Text = "",
        };
        PauseButton.Click += PauseButton_Click;

        var DifficultyButton = new Button(DifficultybuttonTexture, DifficultybuttonFont)
        {
            Position = new Vector2(255, 455),
            Text = "Play Style",
        };
        DifficultyButton.Click += DifficultyButton_Click;

        var MultiplayerButton = new Button(DifficultybuttonTexture, DifficultybuttonFont)
        {
            Position = new Vector2(480, 455),
            Text = "Player 2",
        };
        MultiplayerButton.Click += MultiplayerButton_Click;

        _componentsPause = new List<Component>()
        {
            HomeButton,
            DifficultyButton,
            PauseButton,
            MultiplayerButton,
        };
        _components = new List<Component>()
        {
            PauseButton,
        };

        ball = new Ball(ballTexture)
        {
            position = new Vector2((screenWidth / 2) - (ballTexture.Width / 2), (screenHeight / 2) - (ballTexture.Height / 2)), //positions the ball in the centre of the screen
            score = score,
        };

        AIplayer = new AIPlayer(player2Texture)
        {
            position = new Vector2(730, (screenHeight / 2) - (playerTexture.Height / 2)),
            speed = 5,
        };

        Player1 = new Player(playerTexture)
        {
            position = new Vector2(20, (screenHeight / 2) - (playerTexture.Height / 2)),
        };

        //load in the sprites
        sprites = new List<Sprite>()
        {
            new Sprite(_content.Load<Texture2D>("Background")),
            ball,
            Player1,
            AIplayer,
        };

        Pause = false;
        Multiplayer = false;
    }
        
        public override void Update(GameTime gameTime)
        {
            if (!Pause)
            {
                foreach (var sprite in sprites)
                {
                    sprite.Update(gameTime, sprites);
                }

                foreach (var component in _components)
                    component.Update(gameTime);

                if (!Multiplayer)
                {
                    AIMove();
                }
                else if (Multiplayer)
                {
                    AIplayer.Player2Move();
                }

                if (score.playerScore == Constant.WIN_SCORE)
                {
                    _game.changeState(new Endgame(_game, _graphicsDevice, _content));
                }
                else if (score.AIscore == Constant.WIN_SCORE)
                {
                    _game.changeState(new EndGameLost(_game, _graphicsDevice, _content));
                }
            }
            else
            {
                foreach (var component in _componentsPause)
                    component.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _graphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();

            foreach (var sprite in sprites) sprite.Draw(_spriteBatch);
            if (!Pause)
            {
                foreach (var component in _components)
                    component.Draw(gameTime, _spriteBatch);
            } else if (Pause)
            {
                foreach (var component in _componentsPause)
                    component.Draw(gameTime, _spriteBatch);
                screenFont.Draw(_spriteBatch);
            }

            score.Draw(_spriteBatch);
            if (ball.speed < 4 && score.playerScore == 0 && score.AIscore == 0)
            {
                screenFont.Draw(_spriteBatch); //drawn for the first 5 seconds 
            }

            _spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public void AIMove()
        {
            if (ball.position.Y > AIplayer.position.Y && ball.position.X > screenWidth / 2)
            {
                AIplayer.velocity.Y = AIplayer.speed;
            }
            else if (ball.position.Y < AIplayer.position.Y && ball.position.X > screenWidth / 2)
            {
                AIplayer.velocity.Y = -AIplayer.speed;
            }
            else if (ball.position.Y == AIplayer.position.Y && ball.position.X > screenWidth / 2)
            {
                AIplayer.velocity.Y = 0;
            }
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            _game.changeState(new MenuState(_game, _graphicsDevice, _content));
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            Pause = !Pause;
        }

        private void MultiplayerButton_Click(object sender, EventArgs e)
        {
            Multiplayer = !Multiplayer;
        }

        private void DifficultyButton_Click(object sender, EventArgs e)
        {

            currentYPosition = Player1.position.Y;
            if (difficultyCase >= 2)
            {
                difficultyCase = 0;
                setDifficulty();
            }
            else
            {
                difficultyCase++;
                setDifficulty();
            }
        }

        public void setDifficulty()
        {
            switch (difficultyCase)
            {
                case 0:
                    Player1.position.X = Constant.DEFENSIVE;
                    Player1.position.Y = currentYPosition;
                    break;
                case 1:
                    Player1.position.X = Constant.BALANCED;
                    Player1.position.Y = currentYPosition;
                    break;
                case 2:
                    Player1.position.X = Constant.ATTACKING;
                    Player1.position.Y = currentYPosition;
                    break;
            }
        }
    }
}



