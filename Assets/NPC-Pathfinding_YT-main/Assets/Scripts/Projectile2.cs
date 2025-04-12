using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Dev.Garnet.YT
{
    public class Projectile2 : MonoBehaviour
    {
        public Rigidbody2D rb;
        public float lifetime = 2f;

        private void Update()
        {
            lifetime -= Time.deltaTime;

            if (lifetime <= 0.0f)
            {
                Destroy(this.gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<NPC_Controller>(out NPC_Controller npc))
            {
                npc.curHealth -= 30;
                Destroy(this.gameObject);
            }
        }
    }
}