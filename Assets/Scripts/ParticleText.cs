using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

public class ParticleText : MonoBehaviour {
    
    public Sprite DisplayImage;

    List<Vector2> _textCoordinateList = new List<Vector2>();

    [Range(300, 500)]
    public int Resolution;
    int _currentResolution;
    ParticlePoint[] _points;
    public Gradient Colors;

    [Range(0, 0.05f)]
    public float TextSize;

    public float[] EaseTimes;
    public EaseType[] EaseTypes;
    // Use this for initialization
    void Start() {
        ReadTextCoordinateFromImage(DisplayImage);

    }

    // Update is called once per frame
    void Update() {
        if (_currentResolution != Resolution) {
            CreatePoints();
            SetOffAllParticlePoints();
        }
    }

    void ReadTextCoordinateFromImage(Sprite image) {
        //read from left to right, from bottom to top
        Color[] textImagePixelArray = image.texture.GetPixels();
        int textHorizontalPixel = image.texture.width;
        int textVerticalPixel = image.texture.height;
        for (int i = 0; i < textImagePixelArray.Length; i++) {
            if (textImagePixelArray[i].a != 0) {
                _textCoordinateList.Add(new Vector2(i % textHorizontalPixel - (textHorizontalPixel - 1) * 0.5f, Mathf.Floor(i / textHorizontalPixel) - (textVerticalPixel - 1) * 0.5f));
            }
        }
    }

    void CreatePoints() {
        _currentResolution = Resolution;
        _points = new ParticlePoint[Resolution];
        PoolManager.Pools["ParticlePool"].DespawnAll();
        for (int i = 0; i < _points.Length; i++) {
            Vector2 cordinate = _textCoordinateList[Random.Range(0, _textCoordinateList.Count)];
            _points[i] = PoolManager.Pools["ParticlePool"].Spawn("point").GetComponent<ParticlePoint>();
            _points[i].OriginPosition = _points[i].transform.parent.position + new Vector3(cordinate.x * TextSize, cordinate.y * TextSize, 0f);
            _points[i].transform.position = Random.insideUnitCircle * 20;
            _points[i].SetColor(Colors.Evaluate(Random.value));
            _textCoordinateList.Remove(cordinate);
        }
    }

    void SetOffAllParticlePoints() {
        for (int i = 0; i < _points.Length; i++) {
            //Vector2 cordinate = _textCoordinateList[Random.Range(0, _textCoordinateList.Count)];
            _points[i].SetOffParticle(EaseTimes[Random.Range(0, EaseTimes.Length)], EaseTypes[Random.Range(0, EaseTypes.Length)]);
        }
    }
}
