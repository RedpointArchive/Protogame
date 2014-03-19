namespace Protogame
{
    using System.Diagnostics.CodeAnalysis;
    using Ninject.Modules;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// The Ninject module to load when using Game Analytics (http://gameanalytics.com) services.
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
    public class ProtogameGAAnalyticsIoCModule : NinjectModule
    {
        /// <summary>
        /// An internal method called by the Ninject module system.
        /// Use kernel.Load&lt;Protogame2DIoCModule&gt; to load this module.
        /// </summary>
        public override void Load()
        {
            this.Bind<IAnalyticsEngine>().To<GameAnalyticsAnalyticsEngine>().InSingletonScope();
        }
    }

    // ReSharper restore InconsistentNaming
}