import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CodeReview } from './code-review';

describe('CodeReview', () => {
  let component: CodeReview;
  let fixture: ComponentFixture<CodeReview>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CodeReview],
    }).compileComponents();

    fixture = TestBed.createComponent(CodeReview);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
