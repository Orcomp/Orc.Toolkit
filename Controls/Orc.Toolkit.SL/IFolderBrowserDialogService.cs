// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFolderBrowserDialogService.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   Defines the IFolderBrowserDialogService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit
{
    /// <summary>
    ///     Interface for custom browse dialogs.
    /// </summary>
    public interface IFolderBrowserDialogService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The show folder browser dialog.
        /// </summary>
        /// <param name="folder">
        /// The folder.
        /// </param>
        /// <param name="showNewFolderButton">
        /// The show new folder button.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="useDescriptionForTitle">
        /// The use description for title.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ShowFolderBrowserDialog(
            ref string folder, 
            bool showNewFolderButton = true, 
            string description = null, 
            bool useDescriptionForTitle = true);

        #endregion
    }
}