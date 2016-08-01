using UnityEngine;
using System.Collections;

public class ParticlePoint : MonoBehaviour {

    public Vector3 OriginPosition;

    bool _atOriginPosition = true;
    float _currentEaseTime;
    EaseType _currentEaseType;

    void Update() {
        Tremble();
    }

    void Tremble() {
        transform.position = OriginPosition + (Vector3)Random.insideUnitCircle * 0.05f;
    }

    void FlyAway() {
        _atOriginPosition = false;
        gameObject.MoveTo(OriginPosition + (Vector3)Random.insideUnitCircle * Random.Range(2.5f, 10f), _currentEaseTime * 0.25f, 0, _currentEaseType);
        gameObject.MoveTo(OriginPosition, _currentEaseTime * 0.25f, _currentEaseTime * 0.25f, _currentEaseType);
        Invoke("FlyBack", _currentEaseTime * 0.5f);
    }

    void FlyBack() {
        _atOriginPosition = true;
    }

    public void SetOffParticle(float time, EaseType easeType) {
        _currentEaseTime = time;
        _currentEaseType = easeType;
        gameObject.MoveTo(OriginPosition, time, 0, easeType);
    }

    public void SetColor(Color color) {
        GetComponent<SpriteRenderer>().color = color;
    }

    void OnMouseOver() {
        if (_atOriginPosition) {
            FlyAway();
        }
    }
}
