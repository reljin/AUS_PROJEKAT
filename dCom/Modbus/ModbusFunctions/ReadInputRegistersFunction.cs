using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read input registers functions/requests.
    /// </summary>
    public class ReadInputRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadInputRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadInputRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            Console.WriteLine("Request started");

            // ModbusReadCommandParameters nam treba

            ModbusReadCommandParameters parameters = CommandParameters as ModbusReadCommandParameters;

            byte[] recVal = new byte[12];
            //Head message

            // Data message

            recVal[0] = BitConverter.GetBytes(parameters.TransactionId)[1];
            recVal[1] = BitConverter.GetBytes(parameters.TransactionId)[0];
            recVal[2] = BitConverter.GetBytes(parameters.ProtocolId)[1];
            recVal[3] = BitConverter.GetBytes(parameters.ProtocolId)[0];
            recVal[5] = BitConverter.GetBytes(parameters.Length)[0];
            recVal[6] = parameters.UnitId;
            recVal[7] = parameters.FunctionCode;
            recVal[8] = BitConverter.GetBytes(parameters.StartAddress)[1];
            recVal[9] = BitConverter.GetBytes(parameters.StartAddress)[0];
            recVal[10] = BitConverter.GetBytes(parameters.Quantity)[1];
            recVal[11] = BitConverter.GetBytes(parameters.Quantity)[0];

            Console.WriteLine("Request ended");
            return recVal;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            Dictionary<Tuple<PointType, ushort>, ushort> dictionary = new Dictionary<Tuple<PointType, ushort>, ushort>();

            int byteCount = response[8];
            ushort startAddress = ((ModbusReadCommandParameters)CommandParameters).StartAddress;

            for (int i = 0; i < byteCount; i += 2)
            {
                ushort value = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(response, 9 + i));
                dictionary.Add(new Tuple<PointType, ushort>(PointType.ANALOG_INPUT, startAddress++), value);
            }

            return dictionary;
        }
    }
}