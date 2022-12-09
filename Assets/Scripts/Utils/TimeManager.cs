using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sugarpepper
{
    public class TimeManager : IManagerBase
    {
        public static TimeManager instance = null;
        public static TimeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TimeManager();
                }
                return instance;
            }
        }

        private int curTime = 0;
        private float tempTime = 0f;

        private List<TimeObject> timeObjects = null;


        public void Init()
        {
            if(timeObjects == null)
            {
                timeObjects = new List<TimeObject>();
            }
            else
            {
                timeObjects.Clear();
            }
        }

        public void Update(float dt) 
        {
            tempTime += dt;
            if(tempTime >= 1)
            {
                tempTime -= 1;
                curTime += 1;

                var it = timeObjects.GetEnumerator();
                while(it.MoveNext())
                {
                    if(it.Current == null)
                    {
                        continue;
                    }

                    it.Current.Refresh?.Invoke();
                }
            }
        }

        public static void AddObject(TimeObject target)
        {
            if(target == null)
            {
                return;
            }
            if (Instance.timeObjects == null)
            {
                Instance.timeObjects = new List<TimeObject>();
            }
            Instance.timeObjects.Add(target);
        }

        public static void DelObject(TimeObject target)
        {
            if (target == null)
            {
                return;
            }

            Instance.timeObjects.Remove(target);
        }

        public static void TimeRefresh(int time)
        {
            Instance.curTime = time;
            Instance.tempTime = 0f;
        }

        public static int GetTime()
        {
            return Instance.curTime;
        }

        public static int GetTimeCompare(int target)
        {
            return target - Instance.curTime;
        }
        public static string GetTimeCompareString(int target)
        {
            return SBFunc.TimeString(GetTimeCompare(target));
        }
    }
}
