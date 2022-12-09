using Sugarpepper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sugarpepper
{
    [Flags]
    public enum eDirectionBit
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8
    }
    public class SBController : MonoBehaviour
    {
        [SerializeField]
        protected bool isRight = false;
        public bool IsRight
        {
            get { return isRight; }
            set { isRight = value; }
        }
        [SerializeField]
        protected float speed = 300f;
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        protected bool move = false;
        public bool IsMove
        {
            get { return move; }
        }
        private eDirectionBit typeBit = eDirectionBit.None;

        public void MoveEnter(eDirectionBit directionBit)
        {
            typeBit |= directionBit;
        }
        public void MoveExit(eDirectionBit directionBit)
        {
            typeBit &= ~directionBit;
        }
        public void MoveAllExit()
        {
            typeBit = eDirectionBit.None;
        }
        public virtual void OnController(float dt, bool direction = true, bool isWalk = false, float Speed = -1)
        {
            move = false;
            if (Speed >= 0)
                speed = Speed;

            if (typeBit.HasFlag(eDirectionBit.Up))
            {
                transform.localPosition += (Vector3.up * (speed * dt));
                move = true;
            }
            if (typeBit.HasFlag(eDirectionBit.Down))
            {
                transform.localPosition += (Vector3.down * (speed * dt));
                move = true;
            }
            if (typeBit.HasFlag(eDirectionBit.Right))
            {
                transform.localPosition += (Vector3.right * (speed * dt));
                move = true;
                if (direction)
                    transform.localScale = new Vector2(isRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
            if (typeBit.HasFlag(eDirectionBit.Left))
            {
                transform.localPosition += (Vector3.left * (speed * dt));
                move = true;
                if (direction)
                    transform.localScale = new Vector2(isRight ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
        }
        public virtual IEnumerator MoveTarget(Vector3 localPos, float distance = 0f, bool direction = true, float Speed = -1)
        {
            if (Speed >= 0)
                speed = Speed;

            if (direction)
            {
                var normal = (transform.localPosition - localPos).normalized;
                if (normal.x > 0)
                    transform.localScale = new Vector2(isRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y);
                else if(normal.x < 0)
                    transform.localScale = new Vector2(isRight ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }

            move = true;
            while (Vector2.Distance(transform.position, localPos) > distance)
            {
                yield return null;
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, localPos, Time.deltaTime * speed);
            }
            move = false;
        }

        public virtual IEnumerator MoveWorldTarget(Vector3 worldPos, float distance = 0f, bool direction = true, float Speed = -1)
        {
            if (Speed >= 0)
                speed = Speed;

            if (direction)
            {
                var normal = (transform.position - worldPos).normalized;
                if (normal.x > 0)
                    transform.localScale = new Vector2(isRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y);
                else if (normal.x < 0)
                    transform.localScale = new Vector2(isRight ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }

            move = true;
            while (Vector2.Distance(transform.position, worldPos) > distance)
            {
                yield return null;
                transform.position = Vector3.MoveTowards(transform.position, worldPos, Time.deltaTime * speed);
            }
            move = false;
        }
    }
}