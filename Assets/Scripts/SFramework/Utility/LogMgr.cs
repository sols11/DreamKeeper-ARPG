using System.Collections;
using System.Collections.Generic;
using System;                                               //C#的核心命名空间
using System.Diagnostics;
using System.IO;                                            //文件读写命名空间
using System.Threading;                                     //多线程命名空间
using UnityEngine;

namespace SFramework
{
    public static class LogMgr
    {
        #region  本类的枚举类型
        /// <summary>
        /// 日志状态（部署模式）
        /// </summary>
        public enum State
        {
            Develop,            //开发模式（输出所有日志内容）
            Speacial,           //指定输出模式（只输出High和Special的日志）
            Deploy,             //部署模式（只输出最核心日志信息，例如严重错误信息，用户登陆账号等）
            Stop                //停止输出模式（不输出任何日志信息）
        };

        /// <summary>
        /// 调试信息的等级（表示调试信息本身的重要程度）
        /// </summary>
        public enum Level
        {
            High,
            Special,
            Low
        }
        #endregion
        /* 核心字段 */
        private static List<string> _LogList= new List<string>();            //Log日志缓存数据
        private static string _LogPath = Application.streamingAssetsPath + @"\Log\Game_Log.txt";  //Log日志文件路径
        private static State _LogState= State.Develop;                     //Log日志状态（部署模式）
        private static int _LogBufferMaxNumber=3;             //Log日志缓存最大容量(设为1的话只要有Log就会写入到文件了)
        private static string LOG_ImportTIPS = "@Important !!! ";
        private static string LOG_WarningTIPS = "Warning ";

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static LogMgr()
        {
        }//Log_end(构造函数)

        /// <summary>
        /// 写数据到文件中，会自动记录Log的时间
        /// </summary>
        /// <param name="writeFileDate">写入的调试信息</param>
        /// <param name="level">重要等级级别</param>
        public static void Write(string writeFileDate, Level level = Level.Low)
        {
            //参数检查
            if (_LogState == State.Stop)
                return;

            if (!string.IsNullOrEmpty(writeFileDate))
            {
                //增加日期与时间
                writeFileDate = _LogState.ToString() + "|" + DateTime.Now.ToShortTimeString() + "|   " + writeFileDate + "\r\n";

                //对于不同的“日志状态”，分特定情形写入文件
                if (level == Level.High)
                {
                    writeFileDate = LOG_ImportTIPS + writeFileDate;
                }
                else if (level == Level.Special)
                {
                    writeFileDate = LOG_WarningTIPS + writeFileDate;
                }
                switch (_LogState)
                {
                    case State.Develop:                                        //开发状态
                        //追加调试信息，写入文件
                        AppendDateToFile(writeFileDate);
                        break;
                    case State.Speacial:                                       //“指定"状态
                        if (level == Level.High || level == Level.Special)
                        {
                            AppendDateToFile(writeFileDate);
                        }
                        break;
                    case State.Deploy:                                         //部署状态
                        if (level == Level.High)
                        {
                            AppendDateToFile(writeFileDate);
                        }
                        break;
                    case State.Stop:                                           //停止输出
                        break;
                    default:
                        break;
                }
            }
        }//Write_end

        /// <summary>
        /// 追加数据到文件
        /// </summary>
        /// <param name="writeFileDate">调试信息</param>
        private static void AppendDateToFile(string writeFileDate)
        {
            if (!string.IsNullOrEmpty(writeFileDate))
            {
                //调试信息数据追加到缓存集合中
                _LogList.Add(writeFileDate);
            }

            //缓存集合数量超过一定指定数量（"_LogBufferMaxNumber"）,则同步到实体文件中。
            if (_LogList.Count % _LogBufferMaxNumber == 0)
            {
                //同步缓存数据信息到实体文件中。
                SyncLogCatchToFile();
            }
        }

        /// <summary>
        /// 创建文件与写入文件
        /// </summary>
        /// <param name="pathAndName">路径与名称</param>
        /// <param name="info"></param>
        private static void CreateFile(string pathAndName, string info)
        {
            //文件流信息
            StreamWriter sw;
            FileInfo t = new FileInfo(pathAndName);
            if (!t.Exists)
            {
                //如果此文件不存在则创建
                sw = t.CreateText();
            }
            else
            {
                //如果此文件存在则打开，在后面接着写
                sw = t.AppendText();
            }
            //以行的形式写入信息
            sw.WriteLine(info);
            //关闭流
            sw.Close();
            //销毁流
            sw.Dispose();
        }

        #region  重要管理方法
        /// <summary>
        /// 同步缓存数据信息到实体文件中，可显式调用
        /// </summary>
        public static void SyncLogCatchToFile()
        {
            if (!string.IsNullOrEmpty(_LogPath))
            {
                foreach (string item in _LogList)
                {
                    CreateFile(_LogPath, item);
                    UnityEngine.Debug.Log(_LogPath);
                }
                //清除日志缓存中所有数据
                ClearLogBufferAllDate();
            }
        }

        /// <summary>
        /// 查询日志缓存中的内容
        /// </summary>
        /// <returns>
        /// 返回缓存中的查询内容
        /// </returns>
        public static List<string> QueryAllDateFromLogBuffer()
        {
            if (_LogList != null)
            {
                return _LogList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 查询日志缓存中实际数量个数
        /// </summary>
        /// <returns>
        /// 返回-1,表示查询失败。
        /// </returns>
        public static int QueryLogBufferCount()
        {
            if (_LogList != null)
            {
                return _LogList.Count;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 清除日志缓存中所有数据
        /// </summary>
        public static void ClearLogBufferAllDate()
        {
            if (_LogList != null)
            {
                //数据全部清空
                _LogList.Clear();
            }
        }

        #endregion
    }
}
