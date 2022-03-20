using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
    /// <summary>
    /// The ChainIK constraint job.
    /// </summary>
    [Unity.Burst.BurstCompile]
    public struct ShoulderIKConstraintJob : IWeightedAnimationJob
    {
        /// <summary>An array of Transform handles that represents the Transform chain.</summary>
        public NativeArray<ReadWriteTransformHandle> chain;
        /// <summary>The Transform handle for the target Transform.</summary>
        public ReadOnlyTransformHandle target;

        /// <summary>The offset applied to the target transform if maintainTargetPositionOffset or maintainTargetRotationOffset is enabled.</summary>
        public AffineTransform targetOffset;

        /// <summary>An array of length in between Transforms in the chain.</summary>
        public NativeArray<float> linkLengths;

        /// <summary>An array of positions for Transforms in the chain.</summary>
        public NativeArray<Vector3> linkPositions;

        /// <summary>The weight for which ChainIK target has an effect on chain (up to tip Transform). This is a value in between 0 and 1.</summary>
        public FloatProperty chainRotationWeight;
        /// <summary>The weight for which ChainIK target has and effect on tip Transform. This is a value in between 0 and 1.</summary>
        public FloatProperty tipRotationWeight;

        /// <summary>CacheIndex to ChainIK tolerance value.</summary>
        /// <seealso cref="AnimationJobCache"/>
        public CacheIndex toleranceIdx;
        /// <summary>CacheIndex to ChainIK maxIterations value.</summary>
        /// <seealso cref="AnimationJobCache"/>
        public CacheIndex maxIterationsIdx;
        /// <summary>Cache for static properties in the job.</summary>
        public AnimationJobCache cache;

        /// <summary>The maximum distance the Transform chain can reach.</summary>
        public float maxReach;

        /// <inheritdoc />
        public FloatProperty jobWeight { get; set; }

        /// <summary>
        /// Defines what to do when processing the root motion.
        /// </summary>
        /// <param name="stream">The animation stream to work on.</param>
        public void ProcessRootMotion(AnimationStream stream) { }

        /// <summary>
        /// Defines what to do when processing the animation.
        /// </summary>
        /// <param name="stream">The animation stream to work on.</param>
        public void ProcessAnimation(AnimationStream stream)
        {
            float w = jobWeight.Get(stream);
            if (w > 0f)
            {
                for (int i = 0; i < chain.Length; ++i)
                {
                    var handle = chain[i];
                    linkPositions[i] = handle.GetPosition(stream);
                    chain[i] = handle;
                }

                int tipIndex = chain.Length - 1;
                if (AnimationRuntimeUtils.SolveFABRIK(ref linkPositions, ref linkLengths, target.GetPosition(stream) + targetOffset.translation,
                    cache.GetRaw(toleranceIdx), maxReach, (int)cache.GetRaw(maxIterationsIdx)))
                {
                    var chainRWeight = chainRotationWeight.Get(stream) * w;
                    for (int i = 0; i < tipIndex; ++i)
                    {
                        var prevDir = chain[i + 1].GetPosition(stream) - chain[i].GetPosition(stream);
                        var newDir = linkPositions[i + 1] - linkPositions[i];
                        newDir.y = Mathf.Max(newDir.y, -0.03f);
                        var rot = chain[i].GetRotation(stream);

                        chain[i].SetRotation(stream, Quaternion.Lerp(rot, QuaternionExt.FromToRotation(prevDir, newDir) * rot, chainRWeight));
                    }
                }

                chain[tipIndex].SetRotation(
                    stream,
                    Quaternion.Lerp(
                        chain[tipIndex].GetRotation(stream),
                        target.GetRotation(stream) * targetOffset.rotation,
                        tipRotationWeight.Get(stream) * w
                        )
                    );
            }
            else
            {
                for (int i = 0; i < chain.Length; ++i)
                    AnimationRuntimeUtils.PassThrough(stream, chain[i]);
            }
        }
    }

    /// <summary>
    /// The ChainIK constraint job binder.
    /// </summary>
    /// <typeparam name="T">The constraint data type</typeparam>
    public class ChainIKConstraintJobBinder<T> : AnimationJobBinder<ShoulderIKConstraintJob, T>
        where T : struct, IAnimationJobData, IChainIKConstraintData
    {
        /// <inheritdoc />
        public override ShoulderIKConstraintJob Create(Animator animator, ref T data, Component component)
        {
            Transform[] chain = ConstraintsUtils.ExtractChain(data.root, data.tip);

            var job = new ShoulderIKConstraintJob();
            job.chain = new NativeArray<ReadWriteTransformHandle>(chain.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            job.linkLengths = new NativeArray<float>(chain.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            job.linkPositions = new NativeArray<Vector3>(chain.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            job.maxReach = 0f;

            int tipIndex = chain.Length - 1;
            for (int i = 0; i < chain.Length; ++i)
            {
                job.chain[i] = ReadWriteTransformHandle.Bind(animator, chain[i]);
                job.linkLengths[i] = (i != tipIndex) ? Vector3.Distance(chain[i].position, chain[i + 1].position) : 0f;
                job.maxReach += job.linkLengths[i];
            }

            job.target = ReadOnlyTransformHandle.Bind(animator, data.target);
            job.targetOffset = AffineTransform.identity;
            if (data.maintainTargetPositionOffset)
                job.targetOffset.translation = data.tip.position - data.target.position;
            if (data.maintainTargetRotationOffset)
                job.targetOffset.rotation = Quaternion.Inverse(data.target.rotation) * data.tip.rotation;

            job.chainRotationWeight = FloatProperty.Bind(animator, component, data.chainRotationWeightFloatProperty);
            job.tipRotationWeight = FloatProperty.Bind(animator, component, data.tipRotationWeightFloatProperty);

            var cacheBuilder = new AnimationJobCacheBuilder();
            job.maxIterationsIdx = cacheBuilder.Add(data.maxIterations);
            job.toleranceIdx = cacheBuilder.Add(data.tolerance);
            job.cache = cacheBuilder.Build();

            return job;
        }

        /// <inheritdoc />
        public override void Destroy(ShoulderIKConstraintJob job)
        {
            job.chain.Dispose();
            job.linkLengths.Dispose();
            job.linkPositions.Dispose();
            job.cache.Dispose();
        }

        /// <inheritdoc />
        public override void Update(ShoulderIKConstraintJob job, ref T data)
        {
            job.cache.SetRaw(data.maxIterations, job.maxIterationsIdx);
            job.cache.SetRaw(data.tolerance, job.toleranceIdx);
        }
    }
}