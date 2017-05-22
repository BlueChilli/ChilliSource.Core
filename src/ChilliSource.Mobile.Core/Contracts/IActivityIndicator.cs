#region License

/*
Licensed to Blue Chilli Technology Pty Ltd and the contributors under the MIT License (the "License").
You may not use this file except in compliance with the License.
See the LICENSE file in the project root for more information.
*/

#endregion

using System;
namespace ChilliSource.Mobile.Core
{
	/// <summary>
	/// Contract for implementation independent display of a waiting/busy indicator
	/// </summary>
	public interface IActivityIndicator
	{
        /// <summary>
        /// Displays the indicator
        /// </summary>
		void Show();

        /// <summary>
        /// Displays the indicator with the specified <paramref name="text"/>
        /// </summary>
        /// <param name="text"></param>
		void Show(string text);

        /// <summary>
        /// Stops displaying the indicator
        /// </summary>
		void Hide();
	}
}