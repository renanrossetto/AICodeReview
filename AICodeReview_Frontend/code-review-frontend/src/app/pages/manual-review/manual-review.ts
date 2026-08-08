import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';

import { CodeReviewResponse } from '../../models/code-review-response';
import { ReviewService } from '../../services/review.service';

@Component({
  selector: 'app-manual-review',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './manual-review.html',
  styleUrls: ['./manual-review.css']
})
export class ManualReview {
  codeInput = '';
  reviewedCode = '';
  loading = false;
  errorMessage = '';
  copied = false;

  constructor(
    private readonly reviewService: ReviewService,
    private readonly changeDetectorRef: ChangeDetectorRef
  ) {}

  sendCode(): void {
    if (!this.codeInput.trim() || this.loading) {
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.reviewedCode = '';
    this.copied = false;

    this.changeDetectorRef.detectChanges();

    console.log('================================');
    console.log('[AI Code Review] INICIANDO REVISÃO MANUAL');
    console.log('[AI Code Review] Código:', this.codeInput);

    this.reviewService
      .reviewCode(this.codeInput.trim())
      .pipe(
        finalize(() => {
          this.loading = false;
          this.changeDetectorRef.detectChanges();
          console.log('[AI Code Review] Revisão manual finalizada.');
        })
      )
      .subscribe({
        next: (response: CodeReviewResponse) => {
          console.log('[AI Code Review] Resposta manual:', response);

          const result = response?.reviewResult;

          if (typeof result !== 'string') {
            this.errorMessage =
              'A API respondeu, mas o campo reviewResult não foi encontrado na resposta.';
            return;
          }

          this.reviewedCode = this.normalizeLineBreaks(result);
          this.changeDetectorRef.detectChanges();
        },
        error: (error) => {
          console.error('[AI Code Review] Erro na revisão manual:', error);
          this.errorMessage =
            'Não foi possível concluir a revisão. Verifique se a API e o Ollama estão em execução e tente novamente.';
          this.changeDetectorRef.detectChanges();
        }
      });
  }

  clearCode(): void {
    if (this.loading) {
      return;
    }

    this.codeInput = '';
    this.reviewedCode = '';
    this.errorMessage = '';
    this.copied = false;
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

  get characterCount(): number {
    return this.codeInput.length;
  }

  get canReview(): boolean {
    return !!this.codeInput.trim() && !this.loading;
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
