using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParticleExtensions {
    public static bool GetIsDone(this ParticleSystem ps) {
        if (ps == null) return true;

        if (ps.isEmitting || ps.particleCount > 0) {
            return false;
        }
        for (int ii = 0; ii < ps.subEmitters.subEmittersCount; ii++) {
            if (ps.subEmitters.GetSubEmitterSystem(ii).particleCount > 0) {
                return false;
            }
        }
        return true;
    }

    public static IEnumerator WaitForDone(this ParticleSystem ps) {
        if (ps.main.loop == true) {
            throw new UnityException("WaitForDone called on looping Particle System!");
        }
        while (!ps.GetIsDone()) {
            yield return new WaitForSeconds(0.1f);
        }
    }

}
