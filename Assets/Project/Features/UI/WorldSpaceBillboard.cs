using UnityEngine;

public class WorldSpaceBillboard : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool useStaticBillboard = true;
    [SerializeField] private bool lockX = false;
    [SerializeField] private bool lockY = false;
    [SerializeField] private bool lockZ = false;

    private Transform _mainCameraTransform;
    private Vector3 _originalRotation;

    private void Start()
    {
        if (Camera.main != null)
            _mainCameraTransform = Camera.main.transform;

        _originalRotation = transform.rotation.eulerAngles;
    }

    // Usamos LateUpdate para asegurar que la cámara ya se haya movido (Cinemachine)
    // antes de que nosotros orientemos el Canvas. Evita el "jittering" o temblor.
    private void LateUpdate()
    {
        if (_mainCameraTransform == null) return;

        if (useStaticBillboard)
        {
            // Opción A: El Canvas siempre es paralelo al plano de la cámara (Recomendado)
            transform.rotation = _mainCameraTransform.rotation;
        }
        else
        {
            // Opción B: El Canvas mira directamente a la posición de la cámara
            transform.LookAt(transform.position + _mainCameraTransform.rotation * Vector3.forward,
                             _mainCameraTransform.rotation * Vector3.up);
        }

        // Aplicar restricciones de ejes si es necesario
        Vector3 currentRotation = transform.rotation.eulerAngles;

        float x = lockX ? _originalRotation.x : currentRotation.x;
        float y = lockY ? _originalRotation.y : currentRotation.y;
        float z = lockZ ? _originalRotation.z : currentRotation.z;

        transform.rotation = Quaternion.Euler(x, y, z);
    }
}