using LinkSolver;

namespace WikiLinkSolver {
    public class Program {
        public static async Task Main() {
            string firstDefinition = "Börek";
            string searchedDefinition = "Midnight";

            var wikiSolver = new WikiSolver(2);

            await wikiSolver.Run(firstDefinition, searchedDefinition); 
        }
    }
}
