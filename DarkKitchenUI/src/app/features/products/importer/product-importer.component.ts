import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { ImporterInfo } from '../../../core/models/product.model';

@Component({
  selector: 'app-product-importer',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-importer.component.html',
  styleUrl: './product-importer.component.css'
})
export class ProductImporterComponent implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly router = inject(Router);

  importers: ImporterInfo[] = [];
  selectedImporter = '';
  selectedFile: File | null = null;
  isLoading = true;
  isUploading = false;
  errorMessage = '';
  successMessage = '';

  showImporterUpload = false;
  importerFile: File | null = null;
  isInstalling = false;
  importerErrorMessage = '';
  importerSuccessMessage = '';

  ngOnInit(): void {
    this.loadImporters();
  }

  private loadImporters(): void {
    this.isLoading = true;
    this.productService.getImporters().subscribe({
      next: list => {
        this.importers = list;
        if (list.length && !this.selectedImporter) this.selectedImporter = list[0].name;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load available importers.';
        this.isLoading = false;
      }
    });
  }

  get expectedExtension(): string | null {
    return this.importers.find(i => i.name === this.selectedImporter)?.extension ?? null;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
    this.successMessage = '';
    this.errorMessage = '';
  }

  upload(): void {
    if (!this.selectedImporter || !this.selectedFile) {
      this.errorMessage = 'Please choose an importer and a file.';
      return;
    }

    this.isUploading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.productService.importProducts(this.selectedImporter, this.selectedFile).subscribe({
      next: res => {
        this.successMessage = res.message ?? `${res.imported} products imported.`;
        this.selectedFile = null;
        this.isUploading = false;
      },
      error: err => {
        this.errorMessage = err?.error?.message ?? err?.error ?? 'Import failed.';
        this.isUploading = false;
      }
    });
  }

  toggleImporterUpload(): void {
    this.showImporterUpload = !this.showImporterUpload;
    this.importerErrorMessage = '';
    this.importerSuccessMessage = '';
    if (!this.showImporterUpload) this.importerFile = null;
  }

  onImporterFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.importerFile = input.files?.[0] ?? null;
    this.importerSuccessMessage = '';
    this.importerErrorMessage = '';
  }

  uploadImporter(): void {
    if (!this.importerFile) {
      this.importerErrorMessage = 'Please choose a .dll file.';
      return;
    }

    this.isInstalling = true;
    this.importerErrorMessage = '';
    this.importerSuccessMessage = '';

    this.productService.uploadImporter(this.importerFile).subscribe({
      next: res => {
        this.importerSuccessMessage = res.message ?? 'Importer uploaded.';
        this.importerFile = null;
        this.isInstalling = false;
        this.loadImporters();
      },
      error: err => {
        this.importerErrorMessage = err?.error?.message ?? err?.error ?? 'Upload failed.';
        this.isInstalling = false;
      }
    });
  }

  back(): void {
    this.router.navigate(['/products']);
  }
}
