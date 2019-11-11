using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RedRunner.Characters;

namespace RedRunner.Target
{

    public class Finish : MonoBehaviour
    {

        [SerializeField]
        private Collider2D m_Collider2D;

        void OnCollisionStay2D(Collision2D collision2D)
        {
            Character character = collision2D.collider.GetComponent<Character>();
            if (character && !character.IsDead.Value)
            {
                bool isTop = false;
                ContactPoint2D mainPoint;
                for (int i = 0; i < collision2D.contacts.Length; i++)
                {
                    if (!isTop)
                    {
                        isTop = collision2D.contacts[i].normal.y < -0.7f && collision2D.contacts[i].normal.y >= -1f;
                    }
                    else
                    {
                        break;
                    }
                }
                if (isTop)
                {
                    FinishCharacter(character);
                }
            }
        }

        public void FinishCharacter(Character target)
        {
            target.Finish();
            // TODO: play finish sound
            //AudioManager.Singleton.PlaySpikeSound(transform.position);
        }

    }

}