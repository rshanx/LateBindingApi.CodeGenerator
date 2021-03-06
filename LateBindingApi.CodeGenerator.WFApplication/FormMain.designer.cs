﻿namespace LateBindingApi.CodeGenerator.WFApplication
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStripComponents = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.toolStripApplication = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemLoadProject = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSaveProject = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFilterAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFilterConflicts = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFilterNoConflicts = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripFind = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTypeLibraries = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemLoadTypeLibrary = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTypeLibSeperator = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripGenerator = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemGenerateCode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.websiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.contextMenuStripComponents.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripComponents
            // 
            this.contextMenuStripComponents.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemEdit});
            this.contextMenuStripComponents.Name = "contextMenuStripComponents";
            this.contextMenuStripComponents.Size = new System.Drawing.Size(104, 26);
            this.contextMenuStripComponents.Text = "Edit";
            // 
            // toolStripMenuItemEdit
            // 
            this.toolStripMenuItemEdit.Name = "toolStripMenuItemEdit";
            this.toolStripMenuItemEdit.Size = new System.Drawing.Size(103, 22);
            this.toolStripMenuItemEdit.Text = "Edit";
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripApplication,
            this.toolStripFilter,
            this.toolStripTypeLibraries,
            this.toolStripGenerator,
            this.toolStripHelp});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(1069, 24);
            this.menuStripMain.TabIndex = 23;
            this.menuStripMain.Text = "Application Menu";
            // 
            // toolStripApplication
            // 
            this.toolStripApplication.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemLoadProject,
            this.menuItemSaveProject,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.toolStripApplication.Name = "toolStripApplication";
            this.toolStripApplication.Size = new System.Drawing.Size(71, 20);
            this.toolStripApplication.Text = "Application";
            // 
            // menuItemLoadProject
            // 
            this.menuItemLoadProject.Name = "menuItemLoadProject";
            this.menuItemLoadProject.Size = new System.Drawing.Size(146, 22);
            this.menuItemLoadProject.Text = "Load Project";
            // 
            // menuItemSaveProject
            // 
            this.menuItemSaveProject.Enabled = false;
            this.menuItemSaveProject.Name = "menuItemSaveProject";
            this.menuItemSaveProject.Size = new System.Drawing.Size(146, 22);
            this.menuItemSaveProject.Text = "Save Project";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // toolStripFilter
            // 
            this.toolStripFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripFilterAll,
            this.toolStripFilterConflicts,
            this.toolStripFilterNoConflicts,
            this.toolStripSeparator2,
            this.toolStripFind});
            this.toolStripFilter.Name = "toolStripFilter";
            this.toolStripFilter.Size = new System.Drawing.Size(43, 20);
            this.toolStripFilter.Text = "Filter";
            // 
            // toolStripFilterAll
            // 
            this.toolStripFilterAll.Checked = true;
            this.toolStripFilterAll.CheckOnClick = true;
            this.toolStripFilterAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripFilterAll.Name = "toolStripFilterAll";
            this.toolStripFilterAll.Size = new System.Drawing.Size(142, 22);
            this.toolStripFilterAll.Text = "All";
            // 
            // toolStripFilterConflicts
            // 
            this.toolStripFilterConflicts.CheckOnClick = true;
            this.toolStripFilterConflicts.Name = "toolStripFilterConflicts";
            this.toolStripFilterConflicts.Size = new System.Drawing.Size(142, 22);
            this.toolStripFilterConflicts.Text = "Conflicts";
            // 
            // toolStripFilterNoConflicts
            // 
            this.toolStripFilterNoConflicts.CheckOnClick = true;
            this.toolStripFilterNoConflicts.Name = "toolStripFilterNoConflicts";
            this.toolStripFilterNoConflicts.Size = new System.Drawing.Size(142, 22);
            this.toolStripFilterNoConflicts.Text = "No Conflicts";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(139, 6);
            // 
            // toolStripFind
            // 
            this.toolStripFind.Name = "toolStripFind";
            this.toolStripFind.Size = new System.Drawing.Size(142, 22);
            this.toolStripFind.Text = "Find ...";
            // 
            // toolStripTypeLibraries
            // 
            this.toolStripTypeLibraries.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemLoadTypeLibrary,
            this.toolStripTypeLibSeperator});
            this.toolStripTypeLibraries.Name = "toolStripTypeLibraries";
            this.toolStripTypeLibraries.Size = new System.Drawing.Size(83, 20);
            this.toolStripTypeLibraries.Text = "TypeLibraries";
            // 
            // menuItemLoadTypeLibrary
            // 
            this.menuItemLoadTypeLibrary.Name = "menuItemLoadTypeLibrary";
            this.menuItemLoadTypeLibrary.Size = new System.Drawing.Size(168, 22);
            this.menuItemLoadTypeLibrary.Text = "Load TypeLibrary";
            // 
            // toolStripTypeLibSeperator
            // 
            this.toolStripTypeLibSeperator.Name = "toolStripTypeLibSeperator";
            this.toolStripTypeLibSeperator.Size = new System.Drawing.Size(165, 6);
            // 
            // toolStripGenerator
            // 
            this.toolStripGenerator.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemGenerateCode});
            this.toolStripGenerator.Name = "toolStripGenerator";
            this.toolStripGenerator.Size = new System.Drawing.Size(68, 20);
            this.toolStripGenerator.Text = "Generator";
            // 
            // menuItemGenerateCode
            // 
            this.menuItemGenerateCode.Enabled = false;
            this.menuItemGenerateCode.Name = "menuItemGenerateCode";
            this.menuItemGenerateCode.Size = new System.Drawing.Size(158, 22);
            this.menuItemGenerateCode.Text = "Generate Code";
            // 
            // toolStripHelp
            // 
            this.toolStripHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.websiteToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutToolStripMenuItem});
            this.toolStripHelp.Name = "toolStripHelp";
            this.toolStripHelp.Size = new System.Drawing.Size(40, 20);
            this.toolStripHelp.Text = "&Help";
            // 
            // websiteToolStripMenuItem
            // 
            this.websiteToolStripMenuItem.Name = "websiteToolStripMenuItem";
            this.websiteToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.websiteToolStripMenuItem.Text = "&Goto Homepage";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(159, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 24);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 

            this.splitContainerMain.Size = new System.Drawing.Size(1069, 465);
            this.splitContainerMain.SplitterDistance = 235;
            this.splitContainerMain.TabIndex = 25;
            this.splitContainerMain.Visible = false;

            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1069, 489);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.menuStripMain);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.contextMenuStripComponents.ResumeLayout(false);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem toolStripApplication;
        private System.Windows.Forms.ToolStripMenuItem menuItemLoadProject;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripHelp;
        private System.Windows.Forms.ToolStripMenuItem websiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItemSaveProject;
        private System.Windows.Forms.ToolStripMenuItem toolStripTypeLibraries;
        private System.Windows.Forms.ToolStripMenuItem menuItemLoadTypeLibrary;
        private System.Windows.Forms.ToolStripMenuItem toolStripGenerator;
        private System.Windows.Forms.ToolStripMenuItem menuItemGenerateCode;
        private System.Windows.Forms.ToolStripSeparator toolStripTypeLibSeperator;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripComponents;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEdit;
        private System.Windows.Forms.ToolStripMenuItem toolStripFilter;
        private System.Windows.Forms.ToolStripMenuItem toolStripFilterAll;
        private System.Windows.Forms.ToolStripMenuItem toolStripFilterConflicts;
        private System.Windows.Forms.ToolStripMenuItem toolStripFilterNoConflicts;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripFind;
    }
}

