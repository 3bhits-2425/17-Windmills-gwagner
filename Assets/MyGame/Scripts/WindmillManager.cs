using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WindmillGameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] windmills;
    [SerializeField] private Slider[] windmillSliders;
    [SerializeField] private GameObject colorTarget;

    private List<Button> lockButtons = new List<Button>();
    private float[] speeds;
    private bool[] locked;
    private int currentIndex = 0;

    [SerializeField] private float acceleration = 100f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float maxSpeed = 255f;

    private void Awake()
    {
        speeds = new float[windmills.Length];
        locked = new bool[windmills.Length];
        FindLockButtons();
    }

    private void FindLockButtons()
    {
        lockButtons.AddRange(FindObjectsOfType<Button>());
    }

    private void Start()
    {
        for (int i = 0; i < lockButtons.Count; i++)
        {
            int index = i;
            lockButtons[i].onClick.AddListener(() => LockWindmill(index));
            lockButtons[i].interactable = (i == currentIndex);
        }
    }

    private void Update()
    {
        if (currentIndex < windmills.Length && !locked[currentIndex])
        {
            if (Input.GetKey(KeyCode.Space))
                speeds[currentIndex] += acceleration * Time.deltaTime;
            else
                speeds[currentIndex] -= deceleration * Time.deltaTime;

            speeds[currentIndex] = Mathf.Clamp(speeds[currentIndex], 0, maxSpeed);

            if (windmillSliders.Length > currentIndex && windmillSliders[currentIndex] != null)
                windmillSliders[currentIndex].value = speeds[currentIndex];
        }
        RotateWindmills();
    }

    private void RotateWindmills()
    {
        for (int i = 0; i < windmills.Length; i++)
            windmills[i].transform.Rotate(0, 0, speeds[i] * Time.deltaTime);
    }

    private void LockWindmill(int index)
    {
        if (index == currentIndex && !locked[index])
        {
            locked[index] = true;
            UpdateColor();
            lockButtons[index].interactable = false;
            currentIndex++;
            if (currentIndex < lockButtons.Count)
                lockButtons[currentIndex].interactable = true;
        }
    }

    private void UpdateColor()
    {
        float r = currentIndex >= 1 ? speeds[0] / maxSpeed : 0;
        float g = currentIndex >= 2 ? speeds[1] / maxSpeed : 0;
        float b = currentIndex >= 3 ? speeds[2] / maxSpeed : 0;
        Color newColor = new Color(r, g, b);
        colorTarget.GetComponent<Renderer>().material.color = newColor;
    }
}
