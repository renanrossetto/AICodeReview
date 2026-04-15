namespace AICodeReview.Common
{
    public static class Messages
    {
        public const string NoCsFilesChanged = "Nenhuma alteracao em arquivos .cs encontrada.";

        public const string PullRequestWarning = "Analise baseada em diff (Pull Request)";

        public const string AiResponseError = "Erro na resposta da IA. StatusCode: {0}. Content: {1}";

        public const string AiException = "O codigo retornou uma excecao: {0}";

        public const string MethodWithManyLines = "Metodo '{0}' muito grande ({1} linhas)";

        public const string ClassWithManyMembers = "Classe '{0}' possui muitos membros ({1})";

        public const string BranchNotFound = "Branch nao encontrada no repositorio.";
    }
}
