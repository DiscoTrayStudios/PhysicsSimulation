using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{

    private static float minPlanetMass = .1f;
    private static float maxPlanetMass = 3f;
    private static float maxPlanetSize = 0.2f;
    static float orbitConstant = 15;
    public static Color temp = Color.red;
    public static string planettemp;

    static public Gravity SpawnNewPlanet(Vector2 position, GameObject prefab)
    {
        GameObject planet = Instantiate(prefab);
        planettemp = prefab.tag;
        Gravity g = planet.GetComponentInChildren<Gravity>();
        planet.transform.position = position;
        g.transform.position = position;
        g.GetComponent<Rigidbody2D>().position = position;

        //float m = minPlanetMass + (Random.value * (maxPlanetMass - minPlanetMass));
        //if (prefab.CompareTag("Black Hole")) { m = 100 + Random.value * 1000; }

        //g.GetComponent<Rigidbody2D>().mass = m;

        float scale = 1f + Random.value * maxPlanetSize;
        planet.transform.localScale = new Vector2(scale, scale);

        Vector2 toCenter = GameManager.Instance.deltaV(g);

        float distToCenter = (GameManager.Instance.centerOfSystem - position).magnitude;

        g.startVelocity =
           Quaternion.Euler(0, 0, 90) * toCenter.normalized *
           orbitConstant / Mathf.Sqrt(distToCenter);
        temp = new Color(1, 1, 1, 1);
        planet.GetComponentInChildren<SpriteRenderer>().color = temp;

        return g;
    }

    static public Color getColor()
    {
        return temp;
    }

    static public string getPrefab()
    {
        return planettemp;
    }

    private void Update()
    {

    }
}
