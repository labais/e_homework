using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class StarryBackground : MonoBehaviour
{
    [SerializeField] private GameObject _starPrefab;
    [SerializeField] private float _followSpeed;
    [SerializeField] private Sprite _star1;
    [SerializeField] private Sprite _star2;

    private float _startingCamPosZ;
    private Transform _cam;
    private Vector3 _origPosition;
    private Transform _transform;
    private List<Transform> _stars;
    private List<SpriteRenderer> _starSpriteRenderers;
    private float _origScale;

    private void Start()
    {
        _transform = transform;
        _cam = Camera.main.transform;
        _startingCamPosZ = _cam.position.z;
        _origPosition = _transform.position;
        _origScale = _starPrefab.transform.localScale.x;

        _starPrefab.SetActive(false);

        //  kameras objektam pielikt plakni, kas brauc kopā ar to un tad uz šīs plaknes lokālajā plaknē salikt zvaigznes

        var origRotation = _transform.rotation;
        _transform.rotation = Quaternion.identity;

        _stars = new List<Transform>();
        _starSpriteRenderers = new List<SpriteRenderer>();
        for (var i = 0; i < 200; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-20, 20 + 1), Random.Range(-1, 1 + 1), Random.Range(00, 80 + 1));
            var star = Instantiate(_starPrefab, pos, Quaternion.identity, _transform);

            //star.transform.LookAt(Camera.main.transform.position, Vector3.up);

            _stars.Add(star.transform);
            _starSpriteRenderers.Add(star.GetComponent<SpriteRenderer>());
        }

        _transform.rotation = origRotation;
        foreach (var star in _stars)
        {
            star.gameObject.SetActive(true);
            star.LookAt(_cam.position, Vector3.up);
        }
    }

    private void FixedUpdate()
    {
        var camDelta = _cam.position.z - _startingCamPosZ;
        _transform.position = _origPosition + Vector3.forward * (camDelta * _followSpeed);

        if (Random.Range(0, 30) == 0)
        {
            var r = Random.Range(0, _stars.Count);
            var starSpriteR = _starSpriteRenderers[r];
            var star = _stars[r];

            const float t = .2f;
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    starSpriteR.sprite = _star2;
                    star.DOScale(_origScale * 2, t);
                })
                .AppendInterval(t)
                .AppendCallback(() =>
                {
                    star.DOScale(_origScale, t / 2);
                })
                .AppendInterval(t / 2)
                .AppendCallback(() =>
                {
                    star.DOScale(_origScale, t);
                    starSpriteR.sprite = _star1;
                });
        }
    }

}