using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Models
{
    /// <summary>
    /// Controls the gamestate, including simulation time and speed, and handles UI updates.
    /// </summary>
    public class GameStateController : MonoBehaviour
    {
        private static bool isPaused = false;

        public static double explorerModeDay;

        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI simulationSpeedText;
        [SerializeField] private Slider simulationSpeedSlider;
        [SerializeField] private Toggle realTimeToggle;

        public static float currentExplorerTimeStep;

        [SerializeField] private GameObject datePanel;

        [SerializeField] private CameraControlV2 cam;

        private int simulationDirection = 1;

        private float scale = 1f;
        private const float epsilon = 1e-6f;

        public static bool GetIsPaused() => isPaused;
        public static double GetExplorerModeDay() => explorerModeDay;

        public void SetDayText(TextMeshProUGUI value) => dayText = value;
        public void SetSilmulationSpeedText(TextMeshProUGUI value) => simulationSpeedText = value;
        public void SetSilmulationSpeedSlider(Slider value) => simulationSpeedSlider = value;
        public void SetRealTimeToggle(Toggle value) => realTimeToggle = value;
        public static float GetCurrentExplorerTimeStep() => currentExplorerTimeStep;

        private void Start()
        {
            UpdateDate(DateTime.UtcNow);
            ColorSpeedText();
            SwitchDirectionToForward();
        }
        private void FixedUpdate()
        {
            if (SimulationModeState.currentSimulationMode == SimulationModeState.SimulationMode.Explorer && !isPaused)
            {
                if (realTimeToggle && realTimeToggle.isOn)
                {
                    currentExplorerTimeStep = Time.deltaTime / (24 * 3600) * simulationDirection;
                }
                else
                {
                    currentExplorerTimeStep = Time.deltaTime * scale * simulationDirection;
                }

                DisplayDate(ComputeDateByCurrentDate(GetExplorerModeDay()));
                explorerModeDay += currentExplorerTimeStep;
            }
        }

        /// <summary>
        /// Pause or Resume the simulation. Color the "Play / Pause" Button red, if the game is paused!
        /// </summary>
        public void PlayPause()
        {
            isPaused = !isPaused;
            ExecuteColoring("Play / Pause", 255, isPaused ? 0 : 255, isPaused ? 0 : 255);
        }

        /// <summary>
        /// Sets the time scale for the simulation.
        /// </summary>
        /// <param name="timeScale">The time scale to set.</param>
        public void SetTimeScale(float timeScale)
        {
            if (SimulationModeState.currentSimulationMode == SimulationModeState.SimulationMode.Sandbox)
            {
                Time.timeScale = timeScale;
            }
            else if (SimulationModeState.currentSimulationMode == SimulationModeState.SimulationMode.Explorer)
            {
                scale = timeScale;
            }
            ColorSpeedText();
        }

        private DateTime ComputeDateByCurrentDate(double daysPassed)
        {
            DateTime currentDate = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            currentDate = currentDate.AddDays(daysPassed);
            return currentDate;
        }

        private void DisplayDate(DateTime date)
        {
            TimeZoneInfo switzerlandTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(date, switzerlandTimeZone);
            dayText.text = localDate.ToString("d MMM yyyy HH:mm:ss");
        }

        private void ColorSpeedText()
        {
            simulationSpeedText.text = simulationSpeedSlider.value.ToString("0") + " days per second";

            float value = simulationSpeedSlider.value;

            if (Mathf.Abs(value - simulationSpeedSlider.maxValue) < epsilon)
            {
                ColorText(simulationSpeedText, 255, 0, 0); // Red
            }
            else if (Mathf.Abs(value - simulationSpeedSlider.minValue) < epsilon)
            {
                ColorText(simulationSpeedText, 0, 255, 0); // Green
            }
            else
            {
                ColorText(simulationSpeedText, 125, 125, 0); // Yellow
            }
        }

        private void ColorText(TextMeshProUGUI text, int r, int g, int b)
        {
            text.color = new Color(r / 255f, g / 255f, b / 255f);
        }

        /// <summary>
        /// Calculates the Julian centuries for a given date.
        /// </summary>
        /// <param name="date">The date to calculate for.</param>
        /// <returns>The Julian centuries.</returns>
        public double CalculateJulianCenturies(DateTime date)
        {
            double JD = CalculateJulianDate(date);
            return (JD - 2451545.0) / 36525.0;
        }

        private double CalculateJulianDate(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            if (month <= 2)
            {
                year -= 1;
                month += 12;
            }

            int A = year / 100;
            int B = 2 - A + (A / 4);
            double JD = Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + date.Day + B - 1524.5;

            return JD + (date.Hour + date.Minute / 60.0 + date.Second / 3600.0) / 24.0;
        }

        /// <summary>
        /// Updates the current date in the explorer mode.
        /// </summary>
        /// <param name="time">The date and time to set.</param>
        public void UpdateDate(DateTime time)
        {
            double T = CalculateJulianCenturies(time);
            explorerModeDay = T * 36525;
        }

        /// <summary>
        /// Set the simulationDirection to 1. The simulationspeed is positive! 
        /// Color the "ForwardButton" Green and the "BackwardButton" have no color!
        /// </summary>
        public void SwitchDirectionToForward()
        {
            simulationDirection = 1;
            if (SimulationModeState.currentSimulationMode == SimulationModeState.SimulationMode.Explorer) ColorButton();
        }

        /// <summary>
        /// Set the simulationDirection to -1. The simulationspeed is negative and the simulation moves backwards! 
        /// Color the "BackwardButton" Green and the "ForwardButton" have no color!
        /// </summary>
        public void SwitchDirectionToReverse()
        {
            simulationDirection = -1;
            if (SimulationModeState.currentSimulationMode == SimulationModeState.SimulationMode.Explorer) ColorButton();
        }

        private void ColorButton()
        {
            ExecuteColoring("ForwardButton", simulationDirection > 0 ? 0 : 255, 255, simulationDirection > 0 ? 0 : 255);
            ExecuteColoring("BackwardButton", simulationDirection < 0 ? 0 : 255, 255, simulationDirection < 0 ? 0 : 255);
        }

        private void ExecuteColoring(String buttonName, int r, int g, int b)
        {
            GameObject.Find(buttonName).GetComponent<Image>().color = new Color(r / 255f, g / 255f, b / 255f);
        }

        /// <summary>
        /// Opens the change date panel and locks the camera controls.
        /// </summary>
        public void OpenChangeDatePanel()
        {
            cam.SetKeyboardLock(true);
            datePanel.SetActive(true);
        }
    }
}
