using SFML.Graphics;
using SFML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.System;
using SFML.Audio;

namespace Snek
{
    class Player
    {
        public float xPos, yPos, xVel, yVel;
        Sprite sprite;
        Keyboard.Key left, right;
        bool isLeft, isRight, wasLeft, wasRight;
        bool leftLast = true;
        float accel = 18;

        float spinTime = 3;
        float spinCounter = 0;
        float spinBack = 250;


        public Player(Texture tex, Keyboard.Key left, Keyboard.Key right)
        {
            sprite = new Sprite(tex);
            sprite.Origin = new Vector2f(256f, 256f);
            this.left = left;
            this.right = right;
        }

        public void Reset(float xPos, float yPos)
        {
            this.xPos = xPos;
            this.yPos = yPos;

            Stop();
        }

        public void Stop()
        {
            xVel = 0;
            yVel = 0;
        }

        public void StartSpin()
        {
            spinCounter = spinTime;
        }

        public bool IsSpinning()
        {
            return spinCounter > 0;
        }

        public bool IsMoving()
        {
            return xVel > 0;
        }

        private void Accelerate(float delta)
        {
            xVel += accel;
        }

        public void Update(float delta)
        {
            spinCounter -= delta;

            if (spinCounter > 0)
            {
                xVel = 0;
                yVel = 0;
                xPos -= (spinBack / spinTime) * delta;

                sprite.Rotation = (spinCounter / spinTime) * 2 * 360;

                return;
            } else
            {
                sprite.Rotation = 0;
            }


            isLeft = Keyboard.IsKeyPressed(left);
            isRight = Keyboard.IsKeyPressed(right);

            bool newLeft = isLeft && !wasLeft;
            bool newRight = isRight && !wasRight;

            xPos += xVel * delta;
            yPos += yVel * delta;

            if (leftLast)
            {
                if (newRight)
                {
                    Accelerate(delta);
                    leftLast = false;
                }
                if (newLeft)
                {
                    Stop();
                }
            } else
            {
                if (newLeft)
                {
                    Accelerate(delta);
                    leftLast = true;
                }
                if (newRight)
                {
                    Stop();
                }
            }

            wasLeft = isLeft;
            wasRight = isRight;
        }

        public void Draw(RenderWindow rw)
        {
            sprite.Position = new SFML.System.Vector2f(xPos, yPos);
            sprite.Scale = new Vector2f(1, leftLast ? -1 : 1);
            rw.Draw(sprite);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            View view = new View(new FloatRect(0, 0, 1920, 1080));

            Random random = new Random();
            bool zPressed, wasZPressed = false;

            SoundBuffer birbSoundBuffer = new SoundBuffer("assets/birb_noise.wav");
            Sound birbSound = new Sound(birbSoundBuffer);

            SoundBuffer cheerSoundBuffer = new SoundBuffer("assets/snek_cheer.wav");
            Sound cheerSound = new Sound(cheerSoundBuffer);

            Font snekFont = new Font("assets/snek_font.ttf");
            List<Text> introText = new List<Text>();
            var title = new Text("Sneks on the plains", snekFont, 128);
            title.FillColor = Color.Black;
            title.Position = new Vector2f(100, 450);
            introText.Add(title);

            var instructions = new Text("Snek 1: Z,X\nSnek 2: Comma, Period\nStart: Space\n\nStop when the bird flies by  .  .  .", snekFont, 42);
            instructions.FillColor = Color.Black;
            instructions.Position = new Vector2f(500, 700);
            instructions.LineSpacing = 1.2f;
            introText.Add(instructions);

            Text winText = new Text("Snek 1 wins !", snekFont, 128);
            winText.FillColor = Color.Black;
            winText.Position = new Vector2f(300, 450);


            bool isTitle = true;
            bool isWin = false;

            Music music = new Music("assets/snek_music.ogg");
            music.Volume = 20;
            music.Play();

            RenderWindow window = new RenderWindow(new VideoMode(1920, 1080), "test");
            CircleShape cs = new CircleShape(100.0f);
            cs.FillColor = Color.Green;
            window.SetActive();

            Texture snekTexture = new Texture("assets/snek.png");
            snekTexture.Smooth = true;

            Texture birbTexture = new Texture("assets/birb.png");
            Sprite birb = new Sprite(birbTexture);
            birb.Origin = new Vector2f(256, 256);
            float xBirb, yBirb, xVelBirb, yVelBirb;
            xBirb = 0;
            yBirb = 0;
            xVelBirb = 100;
            yVelBirb = -900;
            bool isBirb = false;
            float birbCounter = random.Next(3,6);

            Texture tGrass1 = new Texture("assets/grass1.png");
            Texture tGrass2 = new Texture("assets/grass2.png");
            Texture tGrass3 = new Texture("assets/grass3.png");

            Texture[] grassTextures = { tGrass1, tGrass2, tGrass3 };

            List<Sprite> grassSprites = new List<Sprite>();

            for (int i = 0; i < 600; i++)
            {
                var grass = new Sprite(grassTextures[random.Next(0, 2)]);
                grass.Position = new Vector2f(i/300.0f * 60000, random.Next(0, 1080));
                grassSprites.Add(grass);
            }

            Player p1 = new Player(snekTexture, Keyboard.Key.Z, Keyboard.Key.X);
            p1.Reset(256, 256);
            Player p2 = new Player(snekTexture, Keyboard.Key.Comma, Keyboard.Key.Period);
            p2.Reset(256, 824);

            Player[] players = { p1, p2 };

            Clock clock = new Clock();

            bool isSpace = false;
            bool wasSpace = false;

            while (window.IsOpen)
            {
                isSpace = Keyboard.IsKeyPressed(Keyboard.Key.Space);
                bool newSpace = isSpace && !wasSpace;
                if (isTitle)
                {
                    window.SetView(window.DefaultView);
                    xBirb = 0;
                    yBirb = 0;
                    xVelBirb = 100;
                    yVelBirb = -900;
                    p1.Reset(256, 256);
                    p2.Reset(256, 824);
                    birbCounter = random.Next(3, 6);

                    window.Clear(Color.White);
                    window.DispatchEvents();
                    
                    foreach (Text text in introText)
                    {
                        window.Draw(text);
                    }

                    window.Display();

                    wasSpace = isSpace;

                    if (newSpace)
                    {
                        isTitle = false;
                    }

                    continue;
                }

                if (isWin)
                {
                    window.SetView(window.DefaultView);
                    window.Clear(Color.White);
                    window.DispatchEvents();

                    window.Draw(winText);
                    if (newSpace)
                    {
                        isWin = false;
                        isTitle = true;
                    }

                    window.Display();

                    wasSpace = isSpace;
                    continue;
                }

                if (p1.xPos - p2.xPos > 1920)
                {
                    isWin = true;
                    winText.DisplayedString = "Snek 1 wins !";
                }
                if (p2.xPos - p1.xPos > 1920)
                {
                    isWin = true;
                    winText.DisplayedString = "Snek 2 wins !";

                }

                Time timeDelta = clock.Restart();
                float delta = timeDelta.AsSeconds();

                window.Clear(Color.White);
                window.DispatchEvents();

                if (Math.Abs(p1.xPos - p2.xPos) > 1408)
                {
                    view.Center = new Vector2f(Math.Min(p1.xPos, p2.xPos) + 704, 540);
                } else
                {
                    view.Center = new Vector2f((p1.xPos + p2.xPos) / 2, 540);
                }
                



                window.SetView(view);



                foreach (Sprite grass in grassSprites)
                {
                    window.Draw(grass);
                }

                foreach (Player p in players)
                {
                    if (isBirb) {
                        if (0 < yBirb && yBirb < 1080)
                        {
                            if (p.IsMoving())
                            {
                                if (!p.IsSpinning())
                                {
                                    p.StartSpin();
                                }
                            }

                            
                        }
                    }


                    p.Update(delta);
                    p.Draw(window);
                }

                if (isBirb)
                {
                    if (yBirb < -256)
                    {
                        isBirb = false;
                    }

                    xBirb += xVelBirb * delta;
                    yBirb += yVelBirb * delta;

                    birb.Position = new Vector2f(xBirb, yBirb);

                    window.Draw(birb);
                }
                else
                {
                    birbCounter -= delta;
                    if (birbCounter < 0)
                    {
                        birbSound.Play();
                        birbCounter = random.Next(3, 6);

                        isBirb = true;
                        xBirb = Math.Max(p1.xPos, p2.xPos);
                        yBirb = 2100;
                    }
                }

                window.Display();
                wasSpace = isSpace;
            }
        }
    }
}
