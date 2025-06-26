using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Microlight.MicroAudio {
    // ****************************************************************************************************
    // Manager for playing MicroInfitnity sound effects
    // ****************************************************************************************************
    public class MicroInfinitySounds {
        readonly List<MicroInfinityInstance> instanceList;

        internal MicroInfinitySounds() { 
            instanceList = new List<MicroInfinityInstance>();

            MicroAudio.UpdateEvent += Update;
        }

        void Update() {
            foreach (MicroInfinityInstance instance in instanceList) {
                instance.Update();
            }
        }

        #region API
        internal MicroInfinityInstance PlayInfinitySound(MicroInfinitySoundGroup infinityGroup, AudioMixerGroup mixerGroup) {
            MicroInfinityInstance newInstance = new MicroInfinityInstance(infinityGroup, mixerGroup);
            instanceList.Add(newInstance);
            newInstance.OnEnd += FinishGroup;
            return newInstance;
        }
        #endregion

        #region Control
        void FinishGroup(MicroInfinityInstance instance) {
            instanceList.Remove(instance);
        }
        #endregion
    }
}