using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using Newtonsoft.Json;


namespace CompleteProject
{
    public class PlayerShooting : MonoBehaviour
    {
        public int damagePerShot = 20;                  // The damage inflicted by each bullet.
        public float timeBetweenBullets = 0.15f;        // The time between each shot.
        public float range = 100f;                      // The distance the gun can fire.



        float timer;                                    // A timer to determine when to fire.
        Ray shootRay = new Ray();                       // A ray from the gun end forwards.
        RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
        int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
        ParticleSystem gunParticles;                    // Reference to the particle system.
        LineRenderer gunLine;                           // Reference to the line renderer.
        AudioSource gunAudio;                           // Reference to the audio source.
        Light gunLight;                                 // Reference to the light component.
		public Light faceLight;								// Duh
        float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.


        void Awake ()
        {
            // Create a layer mask for the Shootable layer.
            shootableMask = LayerMask.GetMask ("Shootable");

            // Set up the references.
            gunParticles = GetComponent<ParticleSystem> ();
            gunLine = GetComponent <LineRenderer> ();
            gunAudio = GetComponent<AudioSource> ();
            gunLight = GetComponent<Light> ();
			//faceLight = GetComponentInChildren<Light> ();
        }


        void Update ()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

#if !MOBILE_INPUT
            // If the Fire1 button is being press and it's time to fire...
			if(Input.GetButton ("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
            {
                // ... shoot the gun.
                Shoot ();
            }
#else
            // If there is input on the shoot direction stick and it's time to fire...
            if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
            {
                // ... shoot the gun
                Shoot();
            }
#endif
            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if(timer >= timeBetweenBullets * effectsDisplayTime)
            {
                // ... disable the effects.
                DisableEffects ();
            }
        }


        public void DisableEffects ()
        {
            // Disable the line renderer and the light.
            gunLine.enabled = false;
			faceLight.enabled = false;
            gunLight.enabled = false;
        }


        void Shoot ()
        {
            // Reset the timer.
            timer = 0f;

            // Play the gun shot audioclip.
            gunAudio.Play ();

            // Enable the lights.
            gunLight.enabled = true;
			faceLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            gunParticles.Stop ();
            gunParticles.Play ();

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
            gunLine.SetPosition (0, transform.position);

            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
            shootRay.origin = transform.position;
            shootRay.direction = transform.forward;

            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
            {
                // Try and find an EnemyHealth script on the gameobject hit.
                EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
                var engine = Manager.instance.engine;
                
                // If the EnemyHealth component exist...
                if(enemyHealth != null)
                {
                    // ... the enemy should take damage.
                    enemyHealth.TakeDamage (damagePerShot, shootHit.point);

                    if (enemyHealth.name == "ZomBunny(Clone)") {
                        // Seta globalmente o agente atacado
                        var agente = "coelho" + enemyHealth.gameObject.GetInstanceID();
                        engine.SetGlobalValue("id", agente);
                        // Crença = serAtacadoCoelho
                        engine.Evaluate("appraise('serAtacado', 'coelho')");

                        if (Manager.elefante == 1) {
                            // Crença = coelhoSofreDanoElefante
                            engine.Execute("gamygdalaEngine.appraiseBelief(1, 'jogador', ['coelhoMorrerElefante'], [0.5], true)");
                            // Debug.Log("Crença coelhoSofreDanoElefante acionada.");
                        }
                        // Crença = coelhoSofreDanoUrso
                        engine.Execute("gamygdalaEngine.appraiseBelief(1, 'jogador', ['coelhoMorrerUrso', 'jogadorPontuar'], [0.5, 0.5], true)");
                        // Debug.Log("Crença coelhoSofreDanoUrso acionada.");

                        // Printa no console o estado emocional do coelho
                        var msgJson = engine.Evaluate("estadoEmocional('coelho')");
                        Manager.setAttributesGain(agente, msgJson.ToString());
                        Debug.Log($"{agente}: " + msgJson);
                        // Debug.Log($"{agente}: " + engine.Evaluate("estadoEmocional('coelho')"));
                    }

                    if (enemyHealth.name == "ZomBear(Clone)") {
                        // Seta globalmente o agente atacado
                        var agente = "urso" + enemyHealth.gameObject.GetInstanceID();
                        engine.SetGlobalValue("id", agente);

                        // Crença = serAtacadoUrso
                        engine.Evaluate("appraise('serAtacado', 'urso')");

                        // Crença = ursoSofreDanoCoelho
                        engine.Execute("gamygdalaEngine.appraiseBelief(1, 'jogador', ['coelhoMorrerUrso', 'jogadorPontuar'], [0.5, 0.5], true)");
                        // Debug.Log("Crença ursoSofreDanoCoelho acionada.");

                        if (Manager.elefante == 1) {
                            // Crença = ursoSofreDanoElefante
                            engine.Execute("gamygdalaEngine.appraiseBelief(1, 'jogador', ['ursoMorrerElefante'], [0.5], true)");
                            // Debug.Log("Crença ursoSofreDanoElefante acionada.");
                        }
                        // Printa no console o estado emocional do coelho
                        Debug.Log($"{agente}: " + engine.Evaluate("estadoEmocional('urso')"));
                    }

                    if (enemyHealth.name == "Hellephant(Clone)") {
                        // Seta globalmente o agente atacado
                        var agente = "elefante" + enemyHealth.gameObject.GetInstanceID();
                        engine.SetGlobalValue("id", agente);
                        // Crença = serAtacadoElefante
                        engine.Evaluate("appraise('serAtacado', 'elefante')");

                        Debug.Log($"{agente}: " + engine.Evaluate("estadoEmocional('elefante')"));
                    }

                }

                // Set the second position of the line renderer to the point the raycast hit.
                gunLine.SetPosition (1, shootHit.point);
            }
            // If the raycast didn't hit anything on the shootable layer...
            else
            {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
            }
        }
    }
}