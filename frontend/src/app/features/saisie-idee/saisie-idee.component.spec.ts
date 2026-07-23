import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SaisieIdeeComponent } from './saisie-idee.component';

describe('SaisieIdeeComponent', () => {
  let component: SaisieIdeeComponent;
  let fixture: ComponentFixture<SaisieIdeeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SaisieIdeeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SaisieIdeeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
