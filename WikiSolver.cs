using System.Text.RegularExpressions;

namespace LinkSolver {
    public record struct Node(string Value, int Depth, List<string> Path);

    public partial class WikiSolver {
        private const string WIKI = "/wiki/";
        private const string WIKI_URL_FORMAT = "https://{0}.wikipedia.org/wiki/";

        private readonly Regex _regex = MyRegex();
        private string? _wikiUrl = string.Empty;

        public uint MaximumDepth;

        public WikiSolver(uint maxDepth = 3) {
            MaximumDepth = maxDepth;
        }

        public async Task Run(string start, string target, string language = "en") {
            if (start == null || start.Length == 0 || target == null || target.Length == 0) {
                return;
            }

            _wikiUrl = string.Format(WIKI_URL_FORMAT, language);
            Console.WriteLine(_wikiUrl);

            var queue = new Queue<Node>();
            queue.Enqueue(new Node(start, 0, new List<string>() { start }));

            var found = await IterateOnQueue(queue, target);
            PrintResult(found);
        }

        private async Task<Node> IterateOnQueue(Queue<Node> queue, string target) {
            Node found = default;
            var client = new HttpClient();

            while (found == default && queue.Any()) {
                var current = queue.Dequeue();

                Console.WriteLine($"Working on {current.Value} in depth {current.Depth}");

                if (current.Depth == MaximumDepth) {
                    continue;
                }

                var wikiPage = await GetWikiPage(client, current.Value);
                found = RunOnLinks(wikiPage, target, current, queue);
            };

            return found;
        }

        private async Task<string> GetWikiPage(HttpClient client, string page) {
            var response = await client.GetAsync($"{_wikiUrl}{page}");
            return await response.Content.ReadAsStringAsync();
        }

        private Node RunOnLinks(string wikiPage, string target, Node current, Queue<Node> queue) {
            foreach (var match in _regex.Matches(wikiPage).ToHashSet<Match>()) {
                var value = match.Groups[1].ToString();

                if (!value.StartsWith(WIKI) || value.Contains('.') || value.Contains(':')) {
                    continue;
                }

                value = value.Replace(WIKI, "", StringComparison.OrdinalIgnoreCase);

                if (value == target) {
                    current.Path.Add(value);
                    return current;
                }

                var list = new List<string>(current.Path) { value };
                queue.Enqueue(new Node(value, current.Depth + 1, list));
            }

            return default;
        }

        private static void PrintResult(Node result) {
            var isFound = result != default;
            Console.WriteLine("Result is: " + (isFound ? "Found" : "Can\'t link"));
            if (isFound) {
                Console.WriteLine($"The path is: {string.Join(" -> ", result.Path)}");
            }
        }

        [GeneratedRegex("href=\"(.*?)\"")]
        private static partial Regex MyRegex();
    }
}
