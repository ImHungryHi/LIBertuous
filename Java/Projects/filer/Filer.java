/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package filer;

import java.awt.BorderLayout;
import java.awt.Dimension;
import java.io.File;
import java.util.Arrays;
import javax.swing.*;

/**
 *
 * @author ImHungryHi
 */

public class Filer {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        File folder = new File("\\\\10.8.10.1\\Books\\Star Wars Comics\\0 Sorted");
        File[] lstFiles = folder.listFiles();
        
        String[] strFiles = folder.list();
        Arrays.sort(strFiles);
        
        JFrame frame = new JFrame("Filer");
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        
        JPanel panel = new JPanel(true);
        panel.setLayout(new BorderLayout());
        
//        JLabel label = new JLabel("Hello World");
//        frame.getContentPane().add(label);
//        panel.add(label);
        
//        JTextArea area = new JTextArea();
//        
//        for (int x = 0; x < strFiles.length; x++) {
//            area.insert(strFiles[x], x);
//        }
//        
//        area.setLineWrap(true);
//        area.setWrapStyleWord(true);
//        area.setSize(200, 200);

        JList<String> list = new JList<String>(strFiles);
        JScrollPane scrollPane = new JScrollPane(list, JScrollPane.VERTICAL_SCROLLBAR_AS_NEEDED, JScrollPane.HORIZONTAL_SCROLLBAR_NEVER);
        panel.add(scrollPane, BorderLayout.WEST);
        
        JList<String> lister = new JList<String>(strFiles);
        JScrollPane scrollPaner = new JScrollPane(lister, JScrollPane.VERTICAL_SCROLLBAR_AS_NEEDED, JScrollPane.HORIZONTAL_SCROLLBAR_NEVER);
        panel.add(scrollPaner, BorderLayout.EAST);

//        JScrollBar scrollBar = new JScrollBar(JScrollBar.VERTICAL, 30, 20, 0, 300);
//        scrollBar.setUnitIncrement(2);
//        scrollBar.setBlockIncrement(1);
//        panel.add(scrollBar, BorderLayout.EAST);

        frame.setContentPane(panel);
        frame.pack();
        frame.setVisible(true);
        frame.setSize(800, 600);
    }
    
}
