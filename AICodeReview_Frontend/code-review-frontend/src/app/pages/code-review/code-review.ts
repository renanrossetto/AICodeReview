import { Component } from '@angular/core';
import { ReviewService } from '../../services/review.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CodeReviewResponse } from '../../models/code-review-response';

@Component({
  selector: 'app-code-review',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './code-review.html',
  styleUrls: ['./code-review.css']
})
export class CodeReview {

  codeInput: string = '';
  reviewedCode: string = '';
  loading: boolean = false;

  constructor(private reviewService: ReviewService) {}

  ngOnInit() {
    this.reviewedCode = 'COMPONENTE FUNCIONANDO';
  }

  sendCode() {

    console.log("Botão clicado");
    console.log("Código enviado:", this.codeInput);

    if (!this.codeInput.trim()) {
      alert('Por favor, insira código para revisar');
      return;
    }

    this.loading = true;
    this.reviewedCode = '';

    this.reviewService.reviewCode(this.codeInput)
      .subscribe({
        next: (response: CodeReviewResponse) => {

          console.log("Resposta da API:", response);

          this.reviewedCode =
            response.reviewResult || 'Nenhuma avaliação retornada';

          this.loading = false;

        },
        error: (err) => {

          console.error("Erro:", err);

          this.reviewedCode = 'Erro ao processar revisão de código.';
          this.loading = false;

        }
      });
  }
}
