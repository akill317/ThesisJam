using UnityEngine;
using System.Collections;

public class ParticlePoint : MonoBehaviour {

    public Vector3 OriginPosition;

    public void SetOffParticle(float time, EaseType easeType) {
        gameObject.MoveTo(OriginPosition, time, 0, easeType);
    }

    public void SetColor(Color color) {
        GetComponent<SpriteRenderer>().color = color;
    }
}
