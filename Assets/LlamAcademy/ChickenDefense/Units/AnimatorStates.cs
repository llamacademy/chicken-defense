using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units
{
    public static class AnimatorStates
    {
        public static readonly int IDLE_ANIMATION = Animator.StringToHash("Idle");
        public static readonly int WALK_ANIMATION = Animator.StringToHash("Walk");
        public static readonly int SPIT_ATTACK = Animator.StringToHash("Spit");
        public static readonly int STOMP_ATTACK = Animator.StringToHash("Stomp");
        public static readonly int EAT = Animator.StringToHash("Eat");
        public static readonly int LAY_EGG = Animator.StringToHash("Lay Egg");
    }
}