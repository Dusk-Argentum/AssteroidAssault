using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Asteroid_Belt_Assault
{
    public class Camera
    {
        public Camera(Viewport viewport)
        {
            Origin = new Vector2(viewport.Width / 2.0f, viewport.Height / 2.0f);
            Zoom = 1.0f;
            ViewPort = viewport;
        }

        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public float Zoom { get; set; }
        public float Rotation { get; set; }
        public Viewport ViewPort { get; set; }

        //position of camera from before shaking
        public Vector2 OriginalPosition;

        //shakingnggnggngnnggggg

        // We only need one Random object no matter how many Cameras we have
        private static readonly Random random = new Random();

        // Are we shaking?
        private bool shaking;

        // The maximum magnitude of our shake offset
        private float shakeMagnitude;

        // The total duration of the current shake
        private float shakeDuration;

        // A timer that determines how far into our shake we are
        private float shakeTimer;

        // The shake offset vector
        private Vector2 shakeOffset;

        /// <summary>
        /// Helper to generate a random float in the range of [-1, 1].
        /// </summary>
        private float NextFloat()
        {
            return (float)random.NextDouble() * 2f - 1f;
        }

        /// <summary>
        /// Shakes the camera with a specific magnitude and duration.
        /// </summary>
        /// <param name="magnitude">The largest magnitude to apply to the shake.</param>
        /// <param name="duration">The length of time (in seconds) for which the shake should occur.</param>
        public void Shake(float magnitude, float duration)
        {
            if (!shaking) //makes it so the original position isn't constantly rewritten while shaking
                OriginalPosition = Position;

            // We're now shaking
            shaking = true;


            // Store our magnitude and duration
            shakeMagnitude = magnitude;
            shakeDuration = duration;

            // Reset our timer
            shakeTimer = 0f;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (shaking)
            {
                // Move our timer ahead based on the elapsed time
                shakeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // If we're at the max duration, we're not going to be shaking anymore
                if (shakeTimer >= shakeDuration)
                {
                    shaking = false;
                    shakeTimer = shakeDuration;
                    Position = OriginalPosition;
                }

                // Compute our progress in a [0, 1] range
                float progress = shakeTimer / shakeDuration;

                // Compute our magnitude based on our maximum value and our progress. This causes
                // the shake to reduce in magnitude as time moves on, giving us a smooth transition
                // back to being stationary. We use progress * progress to have a non-linear fall 
                // off of our magnitude. We could switch that with just progress if we want a linear 
                // fall off.
                float magnitude = shakeMagnitude * (1f - (progress));

                // Generate a new offset vector with three random values and our magnitude
                shakeOffset = new Vector2(NextFloat(), NextFloat()) * magnitude;

                // If we're shaking, add our offset to our position and target
                Position += shakeOffset;
            }
            //gradually returns to original position, but looks weird and might be better to snap back to original position
            //sloppy
            else if (Position.X > OriginalPosition.X ||   
                Position.X < OriginalPosition.X || 
                Position.Y > OriginalPosition.Y || 
                Position.Y < OriginalPosition.Y)
                {
                    if (Position.X - OriginalPosition.X < 3 && Position.X - OriginalPosition.X > -3 && Position.Y - OriginalPosition.Y < 3 && Position.Y - OriginalPosition.Y > -3)
                    {
                        Position = OriginalPosition;
                    }
                    else
                    {
                        if (Position.X > OriginalPosition.X || Position.X < OriginalPosition.X)
                            Position += new Vector2(((OriginalPosition.X - Position.X) / Math.Abs(OriginalPosition.X - Position.X)) * 3, 0);
                        if (Position.Y > OriginalPosition.Y || Position.Y < OriginalPosition.Y)
                            Position += new Vector2(0, ((OriginalPosition.Y - Position.Y) / Math.Abs(OriginalPosition.Y - Position.Y)) * 3);
                    }
                }
        }

        public Matrix GetViewMatrix(Vector2 parallax)
        {
            // To add parallax, simply multiply it by the position
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0.0f)) *
                // The next line has a catch. See note below.
                   Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(Zoom, Zoom, 1) *
                   Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }
    }
}