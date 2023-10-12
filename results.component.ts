import { Component } from '@angular/core';
import { SentenceDataService } from 'src/app/services/sentence-data.service';

@Component({
  selector: 'app-results',
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.css']
})
export class ResultsComponent {


  output: string;

  constructor(private sentence: SentenceDataService) {

    this.output = sentence.finalResult;

  }


}
