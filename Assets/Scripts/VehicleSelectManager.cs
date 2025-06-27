using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleSelectManager : MonoBehaviour
{
    public GameObject toRotate; // Obiekt (rodzic), który będzie rotował, zawierając aktualny pojazd
    public float rotateSpeed;   // Prędkość rotacji obiektu toRotate

    public vehicleList listOfVehicles; // ScriptableObject z listą prefabów pojazdów
    public int vehiclePointer = 0; // Wskaźnik na aktualnie wybrany pojazd w liście

    public static GameObject SelectedCarPrefab; // Statyczna zmienna do przechowywania wybranego prefaba dla innych scen

    private GameObject currentDisplayedVehicle; // Referencja do aktualnie wyświetlanego pojazdu w scenie wyboru

    private void Awake()
    {
        // Wczytaj ostatnio wybrany pojazd z PlayerPrefs. Domyślnie 0.
        vehiclePointer = PlayerPrefs.GetInt("VehiclePointer", 0);

        // Zawsze upewnij się, że wskaźnik jest w prawidłowym zakresie,
        // zwłaszcza po wczytaniu z PlayerPrefs, gdy lista mogła się zmienić.
        if (listOfVehicles == null || listOfVehicles.vehicles == null || listOfVehicles.vehicles.Count == 0)
        {
            Debug.LogError("VehicleSelectManager: 'listOfVehicles' is not assigned or is empty! Please assign a VehicleList ScriptableObject and add vehicles to it.", this);
            this.enabled = false; // Wyłącz skrypt, jeśli nie ma pojazdów do wyświetlenia
            return;
        }
        vehiclePointer = Mathf.Clamp(vehiclePointer, 0, listOfVehicles.vehicles.Count - 1);

        // Wyświetl początkowy pojazd
        DisplayCurrentVehicle();
    }

    private void FixedUpdate()
    {
        // Rotuj obiekt toRotate, jeśli jest przypisany
        if (toRotate != null)
        {
            toRotate.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }

    public void rightButton()
    {
        if (listOfVehicles == null || listOfVehicles.vehicles.Count == 0) return;

        vehiclePointer++;
        // Jeśli wskaźnik przekroczy ostatni element, wróć na początek (cykl)
        if (vehiclePointer >= listOfVehicles.vehicles.Count)
        {
            vehiclePointer = 0; 
        }

        PlayerPrefs.SetInt("VehiclePointer", vehiclePointer); // Zapisz nowy wskaźnik
        DisplayCurrentVehicle(); // Wyświetl nowy pojazd
    }

    public void leftButton()
    {
        if (listOfVehicles == null || listOfVehicles.vehicles.Count == 0) return;

        vehiclePointer--;
        // Jeśli wskaźnik spadnie poniżej pierwszego elementu, przejdź na koniec (cykl)
        if (vehiclePointer < 0)
        {
            vehiclePointer = listOfVehicles.vehicles.Count - 1; 
        }

        PlayerPrefs.SetInt("VehiclePointer", vehiclePointer); // Zapisz nowy wskaźnik
        DisplayCurrentVehicle(); // Wyświetl nowy pojazd
    }

    // Centralna funkcja do wyświetlania/przełączania pojazdów
    private void DisplayCurrentVehicle()
    {
        // 1. Usuń poprzednio wyświetlany pojazd, jeśli istnieje
        // Używamy naszej prywatnej referencji currentDisplayedVehicle,
        // co jest o wiele bardziej wydajne i bezpieczne niż FindGameObjectWithTag.
        if (currentDisplayedVehicle != null)
        {
            Destroy(currentDisplayedVehicle);
        }

        // 2. Sprawdź, czy toRotate jest przypisane
        if (toRotate == null)
        {
            Debug.LogError("VehicleSelectManager: 'toRotate' GameObject is not assigned! Cannot display vehicle.", this);
            return;
        }

        // 3. Ustaw statyczny prefab dla użycia w innych scenach
        SelectedCarPrefab = listOfVehicles.vehicles[vehiclePointer];

        // 4. Instancjonuj nowy pojazd jako dziecko obiektu toRotate
        // (Instantiate z rodzicem domyślnie ustawi localPosition na zero i lokalną rotację na prefabową)
        currentDisplayedVehicle = Instantiate(listOfVehicles.vehicles[vehiclePointer], toRotate.transform);
        
        // 5. Upewnij się, że lokalna pozycja i rotacja są zawsze resetowane
        // To eliminuje problem "starej pozycji" lub rotacji
        currentDisplayedVehicle.transform.localPosition = Vector3.zero;
        
        // Ta linia ustawia lokalną rotację.
        // Jeśli chcesz, aby pojazd zawsze zaczynał z określoną rotacją Y (np. 220 stopni),
        // to ta linia jest OK. Jeśli ma przyjmować domyślną rotację prefabu, zakomentuj ją.
        currentDisplayedVehicle.transform.localRotation = Quaternion.Euler(0f, 220f, 0f); 

        // 6. Upewnij się, że instancjonowany pojazd ma tag "Player"
        // (Bardzo ważne dla CameraOrbit i innych skryptów, które szukają gracza)
        if (!currentDisplayedVehicle.CompareTag("Player"))
        {
            currentDisplayedVehicle.tag = "Player";
        }

        // 7. Opcjonalnie: Wyłączaj Rigidbody i WheelColliders w scenie wyboru
        // Aby uniknąć interakcji fizycznych w menu
        Rigidbody rb = currentDisplayedVehicle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true; // Pojazd będzie nieruchomy, nie będzie reagował na siły
        }
        WheelCollider[] wheelColliders = currentDisplayedVehicle.GetComponentsInChildren<WheelCollider>();
        foreach (WheelCollider wc in wheelColliders)
        {
            wc.enabled = false; // Wyłącz kolizje kół
        }
    }


    public void startGameButton()
    {
        // Upewnij się, że SelectedCarPrefab jest ustawiony na ten wybrany,
        // choć DisplayCurrentVehicle już to robi, to jest dodatkowe zabezpieczenie.
        vehiclePointer = PlayerPrefs.GetInt("VehiclePointer", 0); 
        SelectedCarPrefab = listOfVehicles.vehicles[vehiclePointer]; 

        string trackSceneToLoad = MenuManager.SelectedTrackSceneName;

        if (!string.IsNullOrEmpty(trackSceneToLoad))
        {
            SceneManager.LoadScene(trackSceneToLoad);
        }
        else
        {
            Debug.LogError("VehicleSelectManager: No track scene was selected in MenuManager! Loading SampleScene as fallback.");
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void backToMainMenuButton()
    {
        SceneManager.LoadScene("mainMenu");
    }
}