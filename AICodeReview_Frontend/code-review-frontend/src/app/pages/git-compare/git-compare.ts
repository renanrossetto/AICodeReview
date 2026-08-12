import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';

import {
  GitBranchReviewRequest,
  GitBranchReviewResponse
} from '../../models/git-branch-review';
import { ReviewService } from '../../services/review.service';

@Component({
  selector: 'app-git-compare',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './git-compare.html',
  styleUrls: ['./git-compare.css']
})
export class GitCompare {
  repositoryPath = '';
  baseBranch = 'master';
  compareBranch = '';

  reviewedCode = '';
  errorMessage = '';
  loading = false;
  copied = false;
  diffSize = 0;

  constructor(
    private readonly reviewService: ReviewService,
    private readonly changeDetectorRef: ChangeDetectorRef
  ) {}

  sendGitReview(): void {
    if (!this.canGitReview) {
      return;
    }

    const request: GitBranchReviewRequest = {
      repositoryPath: this.repositoryPath.trim(),
      baseBranch: this.baseBranch.trim(),
      compareBranch: this.compareBranch.trim()
    };

    this.loading = true;
    this.errorMessage = '';
    this.reviewedCode = '';
    this.copied = false;
    this.diffSize = 0;

    this.changeDetectorRef.detectChanges();

    console.log('================================');
    console.log('[AI Code Review] INICIANDO GIT COMPARE');
    console.log('[AI Code Review] Request:', request);

    this.reviewService
      .reviewGitBranch(request)
      .pipe(
        finalize(() => {
          this.loading = false;
          this.changeDetectorRef.detectChanges();
          console.log('[AI Code Review] Git Compare finalizado.');
        })
      )
      .subscribe({
        next: (response: GitBranchReviewResponse) => {
          console.log('[AI Code Review] Resposta Git Compare:', response);

          if (!response) {
            this.errorMessage = 'A API não retornou nenhuma resposta.';
            return;
          }

          if (response.message) {
            this.reviewedCode = this.normalizeLineBreaks(response.message);
            this.diffSize = 0;
            this.changeDetectorRef.detectChanges();
            return;
          }

          if (typeof response.aiReview !== 'string') {
            this.errorMessage =
              'A API respondeu, mas o campo aiReview não foi encontrado na resposta.';
            return;
          }

          this.diffSize = response.diffSize ?? 0;
          this.reviewedCode = this.normalizeLineBreaks(response.aiReview);

          console.log('[AI Code Review] Diff size:', this.diffSize);
          console.log('[AI Code Review] Resultado:', this.reviewedCode);

          this.changeDetectorRef.detectChanges();
        },
        error: (error) => {
          console.error('[AI Code Review] Erro no Git Compare:', error);
          this.errorMessage =
            'Não foi possível realizar a revisão das branches. Verifique o caminho do repositório, as branches e se a API está em execução.';
          this.changeDetectorRef.detectChanges();
        }
      });
  }

  clearGitReview(): void {
    if (this.loading) {
      return;
    }

    this.repositoryPath = '';
    this.baseBranch = 'master';
    this.compareBranch = '';
    this.reviewedCode = '';
    this.errorMessage = '';
    this.copied = false;
    this.diffSize = 0;
  }

  async copyResult(): Promise<void> {
    if (!this.reviewedCode) {
      return;
    }

    try {
      await navigator.clipboard.writeText(this.reviewedCode);
      this.copied = true;

      setTimeout(() => {
        this.copied = false;
        this.changeDetectorRef.detectChanges();
      }, 1800);
    } catch (error) {
      console.error('[AI Code Review] Erro ao copiar resultado:', error);
      this.copied = false;
    }
  }

  get canGitReview(): boolean {
    return (
      !!this.repositoryPath.trim() &&
      !!this.baseBranch.trim() &&
      !!this.compareBranch.trim() &&
      !this.loading
    );
  }

  private normalizeLineBreaks(value: string): string {
    return value
      .replace(/\\r\\n/g, '\n')
      .replace(/\\n/g, '\n')
      .replace(/\\r/g, '\r')
      .replace(/\\t/g, '\t')
      .replace(/\r\n/g, '\n')
      .replace(/\r/g, '\n');
  }
}
