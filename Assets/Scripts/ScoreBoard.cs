using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[RequireComponent(typeof(UIDocument))]
public class ScoreBoard : MonoBehaviour
{
    [SerializeField] private List<GameObject> _players;
    [SerializeField] private VisualTreeAsset _scoreTemplate;


    private VisualElement _container;


    private void Start()
    {
        _container = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Container");

        for (int i = 0; i < _players.Count; i++)
        {
            AddPlayer(_players[i]);
        }
    }


    private void AddPlayer(GameObject player)
    {
        if (!player.TryGetComponent(out IScore score)) { return; }

        VisualElement newPlayerScore = _scoreTemplate.CloneTree();

        Label label = newPlayerScore.Q<Label>("Score");
        label.text = score.Score.ToString();
        score.OnScoreChange += (int newScore) =>
        {
            label.text = newScore.ToString();
        };

        IColor color = score as IColor;
        if (color != null)
        {
            label.style.color = color.Color;
        }

        newPlayerScore.style.flexGrow = 1;
        _container.Add(newPlayerScore);
    }
}
