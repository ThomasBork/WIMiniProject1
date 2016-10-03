namespace JaccardSim
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearchQuery = new System.Windows.Forms.TextBox();
            this.txtSearchResult = new System.Windows.Forms.TextBox();
            this.txtTFIDFSearch = new System.Windows.Forms.Button();
            this.btnCrawl = new System.Windows.Forms.Button();
            this.btnIndex = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(604, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(185, 23);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "Boolean Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearchQuery
            // 
            this.txtSearchQuery.Location = new System.Drawing.Point(203, 12);
            this.txtSearchQuery.Name = "txtSearchQuery";
            this.txtSearchQuery.Size = new System.Drawing.Size(395, 20);
            this.txtSearchQuery.TabIndex = 1;
            // 
            // txtSearchResult
            // 
            this.txtSearchResult.Location = new System.Drawing.Point(203, 38);
            this.txtSearchResult.Multiline = true;
            this.txtSearchResult.Name = "txtSearchResult";
            this.txtSearchResult.Size = new System.Drawing.Size(395, 423);
            this.txtSearchResult.TabIndex = 2;
            // 
            // txtTFIDFSearch
            // 
            this.txtTFIDFSearch.Location = new System.Drawing.Point(604, 41);
            this.txtTFIDFSearch.Name = "txtTFIDFSearch";
            this.txtTFIDFSearch.Size = new System.Drawing.Size(185, 23);
            this.txtTFIDFSearch.TabIndex = 3;
            this.txtTFIDFSearch.Text = "tf*-idf Search";
            this.txtTFIDFSearch.UseVisualStyleBackColor = true;
            this.txtTFIDFSearch.Click += new System.EventHandler(this.txtTFIDFSearch_Click);
            // 
            // btnCrawl
            // 
            this.btnCrawl.Location = new System.Drawing.Point(12, 12);
            this.btnCrawl.Name = "btnCrawl";
            this.btnCrawl.Size = new System.Drawing.Size(185, 23);
            this.btnCrawl.TabIndex = 4;
            this.btnCrawl.Text = "Start Crawling";
            this.btnCrawl.UseVisualStyleBackColor = true;
            this.btnCrawl.Click += new System.EventHandler(this.btnCrawl_Click);
            // 
            // btnIndex
            // 
            this.btnIndex.Location = new System.Drawing.Point(12, 41);
            this.btnIndex.Name = "btnIndex";
            this.btnIndex.Size = new System.Drawing.Size(185, 23);
            this.btnIndex.TabIndex = 5;
            this.btnIndex.Text = "Start Indexing";
            this.btnIndex.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 473);
            this.Controls.Add(this.btnIndex);
            this.Controls.Add(this.btnCrawl);
            this.Controls.Add(this.txtTFIDFSearch);
            this.Controls.Add(this.txtSearchResult);
            this.Controls.Add(this.txtSearchQuery);
            this.Controls.Add(this.btnSearch);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearchQuery;
        private System.Windows.Forms.TextBox txtSearchResult;
        private System.Windows.Forms.Button txtTFIDFSearch;
        private System.Windows.Forms.Button btnCrawl;
        private System.Windows.Forms.Button btnIndex;
    }
}