namespace Protogame
{
    using System.Diagnostics.CodeAnalysis;
    using Protoinject;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// The Protoinject module to load when using Game Analytics (http://gameanalytics.com) services.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Alternative providers can create their own IoC module and bind
    /// to the same interfaces (IAnalyticsEngine) to target game analytics
    /// at a different service.
    /// </para>
    /// <para>
    /// You should ensure that IAnalyticsEngine is bound in a singleton scope.
    /// </para>
    /// </remarks>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    public class ProtogameGAAnalyticsIoCModule : IProtoinjectModule
    {
        /// <summary>
        /// An internal method called by the Protoinject module system.
        /// Use kernel.Load&lt;Protogame2DIoCModule&gt; to load this module.
        /// </summary>
        public void Load(IKernel kernel)
        {
            kernel.Bind<IAnalyticsEngine>().To<GameAnalyticsAnalyticsEngine>().InSingletonScope();
        }
    }
    // ReSharper restore InconsistentNaming
}