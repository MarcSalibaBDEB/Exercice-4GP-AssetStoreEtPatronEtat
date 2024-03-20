using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faireAnimerSkeleton : MonoBehaviour
{
    [SerializeField] private Collider cibleCollider;
    [SerializeField] private float _vitesse;
    [SerializeField] private float _vitesseRotation;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool attaque = Input.GetKeyDown(KeyCode.A);
        
        animator.SetBool("Attack", attaque);

        if (Input.GetMouseButtonDown(0))
        {
            Vector3? vecteurPoint = DeterminerClic();

            if (vecteurPoint.HasValue)
            {
                animator.SetBool("Walk", true);
                Vector3 positionFinale = new Vector3(vecteurPoint.Value.x, transform.localPosition.y, vecteurPoint.Value.z);
                StartCoroutine(DeplacerSquelette(positionFinale));
                StartCoroutine(TournerSquelette(positionFinale));
            }
        }
    }

    private Vector3? DeterminerClic()
    {
        Vector3 posSouris = Input.mousePosition;


        Ray ray = Camera.main.ScreenPointToRay(posSouris);
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == cibleCollider)
            {
                return hit.point;
            }
        }

        return null;
    }

    private IEnumerator DeplacerSquelette(Vector3 destination)
    {
        animator.SetBool("Walk", true);

        float pourcentageMouvement = 0.0f; // Lerp fonctionne avec un pourcentage
        Vector3 positionDepart = transform.position;
        float distance = Vector3.Distance(destination, positionDepart);

        while (pourcentageMouvement <= 1.0f)
        {
            pourcentageMouvement += Time.deltaTime * _vitesse / distance;
            Vector3 nouvellePosition = Vector3.Lerp(positionDepart, destination, pourcentageMouvement);
            transform.position = nouvellePosition;
            yield return new WaitForEndOfFrame();
        }
        animator.SetBool("Walk", false);
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator TournerSquelette(Vector3 destination)
    {
        Vector3 directionRotation = Vector3.Normalize(destination - transform.position);
        Quaternion rotationInitiale = transform.rotation;
        Quaternion rotationCible = Quaternion.LookRotation(directionRotation, Vector3.up);

        float pourcentageRotation = 0.0f;
        float angle = Quaternion.Angle(rotationInitiale, rotationCible);

        while (pourcentageRotation <= 1.0f)
        {
            pourcentageRotation += Time.deltaTime * _vitesseRotation / angle;
            Quaternion rotation = Quaternion.Slerp(rotationInitiale, rotationCible, pourcentageRotation);
            transform.rotation = rotation;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
    }
}
