export interface GitBranchReviewRequest {
  repositoryPath: string;
  baseBranch: string;
  compareBranch: string;
}

export interface GitBranchReviewResponse {
  diffSize?: number;
  aiReview?: string;
  message?: string;
}
