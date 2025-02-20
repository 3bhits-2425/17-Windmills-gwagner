using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WindmillManager : MonoBehaviour
{
    [SerializeField] private GameObject[] windmills;
    [SerializeField] private GameObject colorTarget;
    private List<Button> lockButtons = new List<Button>();
    private List<Slider> windmillSliders = new List<Slider>();
    private List<WindmillDynamicSpeed> windmillScripts = new List<WindmillDynamicSpeed>();
    private bool[] locked;
    private int currentIndex = 0;

    private void Awake()
    {
        locked = new bool[windmills.Length];
        FindWindmillComponents();
    }

    private void FindWindmillComponents()
    {
        foreach (GameObject windmill in windmills)
        {
            WindmillDynamicSpeed windmillScript = windmill.GetComponentInChildren<WindmillDynamicSpeed>();
            if (windmillScript != null)
            {
                windmillScripts.Add(windmillScript);
            }
            else
            {
                Debug.LogError($"WindmillDynamicSpeed nicht gefunden auf {windmill.name}");
            }

            Button button = windmill.GetComponentInChildren<Button>();
            if (button != null)
            {
                lockButtons.Add(button);
                Debug.Log($"Button für {windmill.name} gefunden: {button.name}");
            }
            else
            {
                Debug.LogError($"Kein Button gefunden in {windmill.name}");
            }

            Slider slider = windmill.GetComponentInChildren<Slider>();
            if (slider != null)
            {
                windmillSliders.Add(slider); 
                Debug.Log($"Slider für {windmill.name} gefunden: {slider.name}");
            }
            else
            {
                Debug.LogError($"Kein Slider gefunden in {windmill.name}");
            }
        }
    }

    private void Start()
    {
        int minCount = Mathf.Min(lockButtons.Count, windmillSliders.Count, windmillScripts.Count);
        if (minCount < windmills.Length)
        {
            Debug.LogError($"Nicht genügend Komponenten gefunden! Windmills: {windmills.Length}, Buttons: {lockButtons.Count}, Sliders: {windmillSliders.Count}, Scripts: {windmillScripts.Count}");
        }

        for (int i = 0; i < minCount; i++)
        {
            int index = i;
            lockButtons[i].onClick.AddListener(delegate { LockWindmill(index); });
            lockButtons[i].interactable = (i == currentIndex);
            Debug.Log($"Button {i} registriert - Interactable: {lockButtons[i].interactable}");

            if (windmillSliders[i] != null)
            {
                windmillSliders[i].interactable = (i == currentIndex);
            }
        }

        EnableCurrentWindmill();
    }

    private void EnableCurrentWindmill()
    {
        for (int i = 0; i < windmillScripts.Count; i++)
        {
            windmillScripts[i].enabled = (i == currentIndex);
        }

        Debug.Log($"Windmühle {currentIndex} aktiviert.");
    }

    private void LockWindmill(int index)
    {
        Debug.Log($"LockWindmill aufgerufen für Index {index}");

        if (index < 0 || index >= locked.Length || index >= windmillSliders.Count || index >= lockButtons.Count || index >= windmillScripts.Count)
        {
            Debug.LogError($"Index {index} ist außerhalb des gültigen Bereichs.");
            return;
        }

        if (index == currentIndex && !locked[index])
        {
            locked[index] = true;
            Debug.Log($"Windmühle {index} gesperrt");
            UpdateColor();
            lockButtons[index].interactable = false;

            // Nach dem Sperren Slider-Wert und Geschwindigkeit einfrieren
            if (windmillSliders[index] != null)
            {
                windmillSliders[index].interactable = false;
            }

            // Sperre die Geschwindigkeit der Windmühle
            windmillScripts[index].LockWindmillSpeed();

            // Synchronisiere die Geschwindigkeit mit dem Rotations-Skript
            if (windmillScripts[index] != null)
            {
                float currentSpeed = windmillScripts[index].GetCurrentSpeed();
                WindmillRotationConstantSpeed rotationScript = windmills[index].GetComponentInChildren<WindmillRotationConstantSpeed>();
                if (rotationScript != null)
                {
                    rotationScript.SetRotationSpeed(currentSpeed);
                }
            }

            currentIndex++;
            Debug.Log($"Neuer currentIndex: {currentIndex}");

            if (currentIndex < lockButtons.Count)
            {
                lockButtons[currentIndex].interactable = true;
                if (windmillSliders[currentIndex] != null)
                {
                    windmillSliders[currentIndex].interactable = true;
                }
                Debug.Log($"Button {currentIndex} ist jetzt klickbar.");
            }

            EnableCurrentWindmill();
        }
    }

    private void UpdateColor()
    {
        float r = currentIndex >= 0 && currentIndex < windmillScripts.Count ? windmillScripts[0].GetNormalizedSpeed() : 0;
        float g = currentIndex >= 1 && currentIndex < windmillScripts.Count ? windmillScripts[1].GetNormalizedSpeed() : 0;
        float b = currentIndex >= 2 && currentIndex < windmillScripts.Count ? windmillScripts[2].GetNormalizedSpeed() : 0;

        Color newColor = new Color(r, g, b);
        colorTarget.GetComponent<Renderer>().material.color = newColor;
    }
}
