// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using SiliconStudio.Core;
using SiliconStudio.Xenko.Particles;
using SiliconStudio.Xenko.Particles.Spawners;

namespace CustomParticles.Particles.Spawners
{
    /// <summary>
    /// A custom implementation for the <see cref="ParticleSpawner"/>.
    /// This spawner continuously spawns 100 particles per second and burst spawns 50 particles in a single frame with one second delays
    /// </summary>
    [DataContract("CustomParticleSpawner")] // Used for serialization, a good practice is to have the data contract have the same name as the class
    [Display("CustomParticleSpawner")]
    public sealed class CustomParticleSpawner : ParticleSpawner
    {
        /// <summary>
        /// Carry over the real part
        /// </summary>
        [DataMemberIgnore]
        private float carryOver;    // Private members do not appear on the Property Grid

        [DataMemberIgnore]
        private float spawnCount;    // Private members do not appear on the Property Grid

        [DataMember(100)]                    // When data is serialized, this attribute decides its priority
        [Display("Number of particles")]    // This is the name which will be displayed on the Property Grid
        public float SpawnCount
        {
            get { return spawnCount; }
            set
            {
                // Notify the emitter that the rate has changed and the total maximum number of particles might need to change too
                MarkAsDirty();
                spawnCount = value;
            }
        }

        [DataMemberIgnore]
        private float burstTimer;    // Private members do not appear on the Property Grid

        [DataMemberIgnore]
        private float burstCount;    // Private members do not appear on the Property Grid

        [DataMember(200)]                    // When data is serialized, this attribute decides its priority
        [Display("Burst particles")]    // This is the name which will be displayed on the Property Grid
        public float BurstCount
        {
            get { return burstCount; }
            set
            {
                // Notify the emitter that the rate has changed and the total maximum number of particles might need to change too
                MarkAsDirty();
                burstCount = value;
            }
        }

        public CustomParticleSpawner()
        {
            spawnCount = 100f;
            burstCount = 50f;
            carryOver = 0;
            burstTimer = 0;
        }

        /// <inheritdoc/>
        public override int GetMaxParticlesPerSecond()
        {
            return (int)Math.Ceiling(SpawnCount) + (int)Math.Ceiling(BurstCount);
        }

        /// <inheritdoc/>
        public override void SpawnNew(float dt, ParticleEmitter emitter)
        {
            // State is handled by the base class. Generally you only want to spawn particle when in active state
            var spawnerState = GetUpdatedState(dt, emitter);
            if (spawnerState != SpawnerState.Active)
                return;


            // Calculate particles per second
            var toSpawn = spawnCount * dt + carryOver;
            var integerPart = (int)Math.Floor(toSpawn);
            carryOver = toSpawn - integerPart;


            // Calculate burst particles
            burstTimer -= dt;
            if (burstTimer < 0)
            {
                burstTimer += 1f;
                integerPart += (int)Math.Floor(BurstCount);
            }


            // Lastly, tell the emitter how many new particles do we want to spawn this frame
            emitter.EmitParticles(integerPart);
        }
    }
}

