using UnityEngine;
using System.Collections;

public class LedgeClimb : MonoBehaviour
{
    [Header("Ledge Detection Settings")]
    [SerializeField] private LayerMask ledgeLayer; // LayerMask om te bepalen wat een 'richel' is
    [SerializeField] private float rayStartVerticalOffset = 0.5f; // Verticale offset van Raycast-startpunten
    [SerializeField] private float rayStartHorizontalOffset = 0.3f; // Horizontale afstand van Raycast-startpunten
    [SerializeField] private float horizontalRayLength = 0.3f; // Lengte van de horizontale Raycast
    [SerializeField] private float verticalRayLength = 0.5f; // Lengte van de verticale Raycast

    [Header("Ledge Grab Settings")]
    public float ledgeGrabSpeed = 10f; // Snelheid waarmee de speler naar het grab-punt beweegt
    public Vector3 climbOffset = new Vector3(0, 1.5f, 0); // Offset waar de speler naartoe klimt

    // De variabelen die je al had
    public Vector3 ledgeGrabTarget;

    private bool isLedgeGrabbing = false;
    private Rigidbody rb;
    private int facingDirection = 1; // 1 voor rechts, -1 voor links
    private bool climbRequested = false;

    // Start is called once before the first execution of Update after the MonoBehaviour created
    void Start()
    {
        // Haal de Rigidbody component op
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Bepaal de kijkrichting op basis van de horizontale input. 
        // Dit is nodig om de Raycasts in de juiste richting te sturen.
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0)
        {
            facingDirection = (horizontalInput > 0) ? 1 : -1;
        }

        if (isLedgeGrabbing)
        {
            // 1. Zwaartekracht uitschakelen en snelheid resetten
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;

            // 2. Beweeg de speler naar de berekende richel-grijppositie
            transform.position = Vector3.MoveTowards(transform.position, ledgeGrabTarget, ledgeGrabSpeed * Time.deltaTime);

            // 3. Check of de speler de 'Up' knop indrukt om te klimmen
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!climbRequested)
                {

                    climbRequested = true;
                    
                    // Start de Coroutine om de klimbeweging uit te voeren
                    StartCoroutine(LedgeClimbRoutine());
                }
            }

            // 4. Optioneel: Laat de richel los met 'Down'
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                StopLedgeGrab();
            }
        }
        else
        {
            // Voer de CheckLedgeGrab alleen uit als de speler valt (negatieve Y-snelheid) en de richel niet vast heeft
            if (rb.linearVelocity.y < -0.1f)
            {
                CheckLedgeGrab();
            }
        }
    }

    // Methode voor het detecteren en initialiseren van de richel grab
    void CheckLedgeGrab()
    {
        // De detectielogica maakt gebruik van 3 Raycasts:
        // 1. Muurdetectie: Raycast horizontaal naar buiten.
        // 2. 'Gap' detectie: Raycast omhoog vanaf de muurhit.
        // 3. Richel hoogte: Raycast omlaag om de precieze Y-coördinaat van de richel te vinden.

        // --- 1. Muurdetectie ---
        Vector3 rayStartPos = transform.position + Vector3.up * rayStartVerticalOffset;
        Vector3 rayDirection = Vector3.right * facingDirection;
        Vector3 wallRayStart = rayStartPos + Vector3.right * facingDirection * rayStartHorizontalOffset;

        RaycastHit wallHit;
        bool wallDetected = Physics.Raycast(wallRayStart, rayDirection, out wallHit, horizontalRayLength, ledgeLayer);

        Debug.DrawRay(wallRayStart, rayDirection * horizontalRayLength, wallDetected ? Color.red : Color.green);

        if (wallDetected)
        {
            // --- 2. 'Gap' detectie (bovenste hoek vrij?) ---
            Vector3 gapRayStart = wallHit.point + Vector3.up * 0.05f + Vector3.right * facingDirection * 0.05f;
            RaycastHit gapHit;
            // Kijken of er GEEN object is direct boven de muurhit
            bool gapDetected = !Physics.Raycast(gapRayStart, Vector3.up, out gapHit, verticalRayLength, ledgeLayer);

            Debug.DrawRay(gapRayStart, Vector3.up * verticalRayLength, gapDetected ? Color.yellow : Color.blue);

            if (gapDetected)
            {
                // --- 3. Richel hoogte detectie (waar is de top van de richel?) ---
                // Start iets voor de muur en kijk omlaag
                Vector3 ledgeRayStart = transform.position + Vector3.up * rayStartVerticalOffset + Vector3.right * facingDirection * (horizontalRayLength + 0.05f);

                RaycastHit ledgeHit;
                // Kijken of er EEN object is omlaag
                if (Physics.Raycast(ledgeRayStart, Vector3.down, out ledgeHit, verticalRayLength, ledgeLayer))
                {
                    Debug.DrawRay(ledgeRayStart, Vector3.down * verticalRayLength, Color.magenta);

                    // De richel is gedetecteerd. Bereken de grab-positie.
                    // X: De muurhit X, minus een offset zodat de speler niet in de muur zit.
                    // Y: De richelhit Y, plus een kleine offset voor de voeten van de speler.
                    ledgeGrabTarget = new Vector3(
                        wallHit.point.x - facingDirection * (transform.localScale.x / 2 + 0.1f),
                        ledgeHit.point.y - (transform.localScale.y / 2) + 0.1f,
                        transform.position.z
                    );

                    // Start de Ledge Grab
                    isLedgeGrabbing = true;
                    // Draai de speler om naar de muur te kijken (optioneel, maar maakt het duidelijker)
                    transform.rotation = Quaternion.Euler(0, facingDirection == 1 ? 90 : -90, 0);

                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.useGravity = false;
                }
            }
        }
    }

    // Coroutine om de klimanimatie/beweging uit te voeren
    IEnumerator LedgeClimbRoutine()
    {
        // 1. De-activeren van de grab status
        isLedgeGrabbing = false;

        // 2. Bereken de eindpositie van de klim
        Vector3 targetPosition = ledgeGrabTarget + climbOffset;

        float startTime = Time.time;
        float duration = 0.5f; // Duur van de klim

        while (Time.time < startTime + duration)
        {
            // Lineaire interpolatie voor een vloeiende beweging
            float t = (Time.time - startTime) / duration;
            transform.position = Vector3.Lerp(transform.position, targetPosition, t);

            yield return null; // Wacht een frame
        }

        // Zorg ervoor dat de speler precies op de targetPositie staat
        transform.position = targetPosition;

        // 3. Reset de status na de klim
        StopLedgeGrab();
    }

    // Reset alle states na een grab/climb
    private void StopLedgeGrab()
    {
        isLedgeGrabbing = false;
        climbRequested = false;
        rb.useGravity = true; // Zwaartekracht weer aan
        // Reset de rotatie naar de standaard
        transform.rotation = Quaternion.identity;

        // Geef de speler een lichte "duw" omhoog na het klimmen, voor een soepelere afwerking
        rb.AddForce(Vector3.up * 2f, ForceMode.VelocityChange);
    }
}