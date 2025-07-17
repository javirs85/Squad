using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class RequestBluetoothPermissions : MonoBehaviour
{
    [Header("Nombre de la escena principal")]
    [SerializeField] string nextScene;

    [Header("Tiempo adicional después de obtener permisos")]
    [SerializeField] float postPermissionDelay = 1.0f;

    void Start()
    {
        StartCoroutine(HandlePermissionsAndLoadScene());
    }

    public IEnumerator HandlePermissionsAndLoadScene()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
            Permission.RequestUserPermission("android.permission.BLUETOOTH_CONNECT");

        if (!Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN"))
            Permission.RequestUserPermission("android.permission.BLUETOOTH_SCAN");

        if (!Permission.HasUserAuthorizedPermission("android.permission.ACCESS_FINE_LOCATION"))
            Permission.RequestUserPermission("android.permission.ACCESS_FINE_LOCATION");


        yield return new WaitForSeconds(postPermissionDelay);

        SceneManager.LoadScene(nextScene);
    }
}
