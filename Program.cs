using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace PS
{
    public class ParticleSystem : Transformable, Drawable
    {
        public ParticleSystem(uint count)
        {
            m_particles = new Particle[count].ToList<Particle>();
            m_verices = new VertexArray(PrimitiveType.Points, count);
            m_lifetime = Time.FromSeconds(0.3f);
            m_emitter = new Vector2f(100, 100);
        }


        private void resetParticle(int index)
        {
            float angle = (new Random().Next(0, 360)) * 3.14f / 180.0f;
            float speed = (new Random().Next(0, 50)) + 50.0f;

            m_particles[index] = new ParticleSystem.Particle
            {
                velocity = new Vector2f((float)(Math.Cos(angle) * speed), (float)(Math.Sin(angle) * speed)),
                lifeTime = Time.FromMilliseconds(new Random().Next(0, 2000) + 1000)
            };

            m_verices[(uint)index] = new Vertex { Position = m_emitter };
        }
        public void setEmitter(Vector2f pos)
        {
            m_emitter = pos;
        }
        public void update(Time elapsed)
        {
            for (int i = 0; i < m_particles.Count; i++)
            {
                Particle particle = m_particles[i];
                particle.lifeTime -= elapsed;

                if (particle.lifeTime < Time.Zero)
                {
                    resetParticle(i);
                }

                m_verices[(uint)i] = new Vertex { Position = m_verices[(uint)i].Position + particle.velocity * elapsed.AsSeconds() };
                float ratio = particle.lifeTime.AsSeconds() / m_lifetime.AsSeconds();
                Vertex vertex = m_verices[(uint)i];
                vertex.Color = new SFML.Graphics.Color(vertex.Color.R, vertex.Color.G, vertex.Color.B, (byte)(ratio * 255));
                m_verices[(uint)i] = vertex;
            }
        }

        void Drawable.Draw(RenderTarget target, RenderStates states)
        {
            states.Transform *= Transform;
            states.Texture = null;
            target.Draw(m_verices, states);
        }

        struct Particle
        {
            public Vector2f velocity;
            public Time lifeTime;
        }

        List<Particle> m_particles;
        VertexArray m_verices;
        Time m_lifetime;
        Vector2f m_emitter;
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            RenderWindow renderWindow = new(new VideoMode((uint)900, (uint)900, (uint)32), "Particle System");
            ParticleSystem particleSystem = new(2000);
            Clock clock = new Clock();
            while (renderWindow.IsOpen)
            {
                Vector2i mouse = Mouse.GetPosition(renderWindow);
                particleSystem.setEmitter(renderWindow.MapPixelToCoords(mouse));

                Time elapsed = clock.Restart();
                particleSystem.update(elapsed);
                
                renderWindow.DispatchEvents();
                renderWindow.Clear(SFML.Graphics.Color.White);
                renderWindow.Draw(particleSystem);
                renderWindow.Display();

                if(Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    particleSystem = new(5000);
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                {
                    renderWindow.Close();
                }
            }

        }
    }
}