using UnityEngine;

namespace CompleteProject
{
    public class EnemyManager : MonoBehaviour
    {
        public PlayerHealth playerHealth;       // Reference to the player's heatlh.
        public GameObject enemy;                // The enemy prefab to be spawned.
        public float spawnTime = 1f;            // How long between each spawn.
        public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.

        private int invoker = 0;

        

        void Start ()
        {
            // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
            // InvokeRepeating("Spawn", spawnTime, spawnTime);
            
            Invoke("Spawn", spawnTime);
            // Invoke("Spawn", spawnTime);
            // Invoke("Spawn", spawnTime);
            // Invoke("Spawn", spawnTime);
        }


        void Spawn ()
        {
            // If the player has no health left...
            if(playerHealth.currentHealth <= 0f)
            {
                // ... exit the function.
                return;
            }

            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range (0, spawnPoints.Length);
            var engine = Manager.instance.engine;

            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
            var guilhermeDelicinhaQueEuAmoMuitoObrigado = Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);

            if (enemy.name == "ZomBunny") {
                var agente = "coelho" + guilhermeDelicinhaQueEuAmoMuitoObrigado.GetInstanceID();
                engine.SetGlobalValue("id", agente);
                engine.Execute("criaAgenteCoelho()");
                Manager.coelho = 1;
                AgentsManager.instance.addActiveAgents(guilhermeDelicinhaQueEuAmoMuitoObrigado);
            }

            if (enemy.name == "ZomBear") {
                var agente = "urso" + guilhermeDelicinhaQueEuAmoMuitoObrigado.GetInstanceID();
                engine.SetGlobalValue("id", agente);
                engine.Execute("criaAgenteUrso()");
                Manager.urso = 1;
                AgentsManager.instance.addActiveAgents(guilhermeDelicinhaQueEuAmoMuitoObrigado);
            }

            if (enemy.name == "Hellephant") {
                var agente = "elefante" + guilhermeDelicinhaQueEuAmoMuitoObrigado.GetInstanceID();
                engine.SetGlobalValue("id", agente);
                engine.Execute("criaAgenteElefante()");
                Manager.elefante = 1;
                AgentsManager.instance.addActiveAgents(guilhermeDelicinhaQueEuAmoMuitoObrigado);
            }
        }
    }
}