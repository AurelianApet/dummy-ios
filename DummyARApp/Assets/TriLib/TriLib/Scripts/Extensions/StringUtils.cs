﻿using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace TriLib {
    /// <summary>
    /// Represents a series of string functions.
    /// </summary>
    public class StringUtils
    {
        /// <summary>
        /// Generates a new name for an object.
        /// </summary>
        /// <returns>Generated name.</returns>
        public static string GenerateUniqueName(object id)
        {
            return id.GetHashCode().ToString(CultureInfo.InvariantCulture);
        }

		/// <summary>
		/// Decompress a GZ compressed string.
		/// </summary>
		/// <returns>The decompressed string.</returns>
		/// <param name="compressedText">String value.</param>
		public static byte[] UnZip(string value)
		{
			byte[] gZipBuffer = Convert.FromBase64String(value);
			using (var memoryStream = new MemoryStream())
			{
				int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
				memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

				var buffer = new byte[dataLength];

				memoryStream.Position = 0;
				using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					gZipStream.Read(buffer, 0, buffer.Length);
				}

				return buffer;
			}
		}
    }
}

