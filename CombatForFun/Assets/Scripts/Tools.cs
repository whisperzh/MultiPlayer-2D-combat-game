using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

namespace My_Tools
{

    public class Tools
    {
       
        /// <summary>
        /// 等interval毫秒然后执行action
        /// </summary>
        /// <param name="interval">等待时间单位为毫秒</param>
        /// <param name="action">执行的操作</param>
        public static void SetTimeOut(double interval, Action action)
        {
            System.Timers.Timer timer = new System.Timers.Timer(interval);
            //实例化Timer类，设置间隔时间；
            timer.Elapsed += new System.Timers.ElapsedEventHandler(delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                timer.Enabled = false;
                action();
            });//到达时间的时候执行事件；
            timer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
        }


        /// <summary>
        /// 设备震动多剧烈，震动几秒
        /// </summary>
        /// <param name="device">要振动的设备/param>
        /// <param name="interval"> 震动时长</param>
        /// <param name="intensity">震动强度</param>
        public static void DeviceViberation(InputDevice device,double interval,Vector2 intensity) {
            device.Vibrate(intensity.x, intensity.y);
            SetTimeOut(1000 * interval, delegate { device.StopVibration(); });
        }


        public static float RollParameterForeachFrame(float xAxis, float scale, float leftSummit, float rightSummit, float end)
        {
            if (xAxis < leftSummit)
                return scale / leftSummit * xAxis;//=y
            else if (xAxis >= leftSummit && xAxis < rightSummit)
                return scale;
            else if (xAxis >= rightSummit && xAxis <= end)
                return -end * scale / (rightSummit - end) +
                    xAxis * scale / (rightSummit - end);
            else
                return 0;
        }

    }
}
