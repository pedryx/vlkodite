using System.Collections;
using System.Text;

using TMPro;

using UnityEngine;

public class QuestItemController : MonoBehaviour
{
    [SerializeField]
    private GameObject subQuestPanelPrefab;

    [field: SerializeField]
    public float Duration { get; private set; } = 1.0f;

    public void AddSubQuest(SubQuest subQuest)
    {
        GameObject subQuestItem = Instantiate(subQuestPanelPrefab, transform);
        var textMeshPro = subQuestItem.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.text = subQuest.Description;

        ChildController.Instance.OnSubQuestDone.AddListener(e =>
        {
            if (e.SubQuest == subQuest)
                StartCoroutine(DoCross(textMeshPro));
        });
    }

    private IEnumerator DoCross(TextMeshProUGUI textMeshPro)
    {
        string originalText = string.Copy(textMeshPro.text);
        int length = CountLength(originalText);
        float timePerCharacter = Duration / length;

        for (int i = 0; i <= length; i++)
        {
            textMeshPro.text = Cross(originalText, i);
            yield return new WaitForSeconds(timePerCharacter);
        }
    }

    private int CountLength(string str)
    {
        bool counting = true;
        int count = 0;

        foreach (char c in str)
        {
            if (c == '<')
            {
                counting = false;
                continue;
            }
            if (c == '>')
            {
                counting = true;
                continue;
            }

            if (counting)
                count++;
        }

        return count;
    }

    private string Cross(string str, int crossLength)
    {
        const string crossStart = "<s>";
        const string crossEnd = "</s>";

        bool ignore = false;
        StringBuilder builder = new();
        int currentCrossLength = 0;
        int crossEndIndex = 0;

        builder.Append(crossStart);


        for (int i = 0; i < str.Length; i++)
        {
            if (currentCrossLength == crossLength)
            {
                crossEndIndex = i;
                builder.Append(crossEnd);
                break;
            }

            char c = str[i];

            if (c == '<')
            {
                ignore = true;
                builder.Append(crossEnd);
            }

            builder.Append(c);

            if (c == '>')
            {
                ignore = false;
                builder.Append(crossStart);
            }

            if (!ignore)
                currentCrossLength++;
        }

        if (crossEndIndex == str.Length - 1)
            builder.Append(crossEnd);
        else
            builder.Append(str[crossEndIndex..]);

        return builder.ToString();
    }
}
