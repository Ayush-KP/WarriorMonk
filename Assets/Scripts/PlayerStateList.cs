using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
   public bool jumping = false;
   public bool dashing = false;
   public bool recoilingX, recoilingY;
   public bool lookingRight = true;
   public bool invincible = false;
   public bool healing = false;
   public bool casting = false;
   public bool cutscene = false;
   public bool alive = true;
}
